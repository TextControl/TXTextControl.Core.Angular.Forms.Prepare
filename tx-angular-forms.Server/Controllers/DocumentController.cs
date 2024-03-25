using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TXTextControl;
using TXTextControl.DocumentServer;
using TXTextControl.Web.MVC.DocumentViewer.Models;

namespace tx_angular_forms.Server.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DocumentController : ControllerBase
	{

		private readonly ILogger<DocumentController> _logger;

		public DocumentController(ILogger<DocumentController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		[Route("Prepare")]
		public DocumentData Prepare()
		{
			// create a new CustomerData object and serialize it to JSON
			var customer = new CustomerData {
				Name = "Smith",
				FirstName = "John",
				Street = "Main Street 1",
				City = "New York",
				Zip = "10001",
				Country = "United States"
			};

			var customerJson = JsonSerializer.Serialize(customer);

			byte[] documentBytes;

			using (var tx = new ServerTextControl()) {
				tx.Create();
				tx.Load("App_Data/template.tx", StreamType.InternalUnicodeFormat);

				// create a new MailMerge object and merge the JSON data
				var merge = new MailMerge {
					TextComponent = tx,
					FormFieldMergeType = FormFieldMergeType.Preselect,
					RemoveEmptyFields = false,
					RemoveEmptyImages = false
				};

				merge.MergeJsonData(customerJson);

				// save the document
				tx.Save(out documentBytes, BinaryStreamType.InternalUnicodeFormat);
			}

			var documentBase64 = Convert.ToBase64String(documentBytes);
			return new DocumentData { Name = "template.pdf", Document = documentBase64 };
		}


		[HttpPost]
		[Route("CustomSign")]
		public string CustomSign([FromBody] SignatureData signatureData)
		{
			byte[] pdfBytes;

			using (var tx = new ServerTextControl()) {
				tx.Create();
				tx.Load("App_Data/template.tx", StreamType.InternalUnicodeFormat);

				// load the signature image into the signature fields
				foreach (var box in signatureData.SignatureBoxes) {
					foreach (SignatureField field in tx.SignatureFields) {
						if (field.Name == box.Name) {
							var stamp = Convert.FromBase64String(signatureData.SignatureImage);

							using (var ms = new MemoryStream(stamp, 0, stamp.Length, false, true)) {
								field.Image = new SignatureImage(ms);
							}
						}
					}
				}

				// fill the form fields with the data
				foreach (TXTextControl.FormField formField in tx.FormFields) {
					foreach (var dataField in signatureData.FormFields) {
						if (dataField.Name == formField.Name) {
							formField.Text = dataField.Value;
							break;
						}
					}
				}

				FlattenFormFields(tx);
				tx.Save(out pdfBytes, BinaryStreamType.AdobePDF);
			}

			return Convert.ToBase64String(pdfBytes);
		}


		// Flatten all form fields in a document
		private void FlattenFormFields(ServerTextControl textControl)
		{
			int fieldCount = textControl.FormFields.Count;

			for (int i = 0; i < fieldCount; i++) {
				TextFieldCollectionBase.TextFieldEnumerator fieldEnum =
				  textControl.FormFields.GetEnumerator();
				fieldEnum.MoveNext();

                TXTextControl.FormField curField = (TXTextControl.FormField)fieldEnum.Current;
				textControl.FormFields.Remove(curField, true);
			}
		}

	}
}