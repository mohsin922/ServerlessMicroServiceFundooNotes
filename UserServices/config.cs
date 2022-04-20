//using Microsoft.Azure.Documents;
//using Microsoft.Azure.Documents.Client;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace UserServices
//{
//    public class config
//    {
//        private IConfiguration _Toolsettings;

//        public config(IConfiguration _Toolsettings)
//        {
//            this._Toolsettings = _Toolsettings;
//        }


//        public void Initialize()
//        {
//            DocumentClient client = new DocumentClient(new Uri(this._Toolsettings["Values:DocDbEndpoint"]), this._Toolsettings["Values:DocDbMasterKey"]);

//            CreateDatabaseIfNotExistsAsync(client).Wait();
//            CreateCollectionIfNotExistsAsync(client).Wait();
//        }

//        private async Task CreateCollectionIfNotExistsAsync(DocumentClient client)
//        {
//            try
//            {
//                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(this._Toolsettings["Values:DataBaseId"]));
//            }
//            catch (DocumentClientException e)
//            {
//                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
//                {
//                    await client.CreateDatabaseAsync(new Microsoft.Azure.Documents.Database { Id = this._Toolsettings["Values:DataBaseId"] });
//                }
//                else
//                {
//                    throw;
//                }
//            }
//        }

//        private async Task CreateDatabaseIfNotExistsAsync(DocumentClient client)
//        {
//            try
//            {
//                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(this._Toolsettings["Values:DataBaseId"], this._Toolsettings["Values:ContainerId"]));
//            }
//            catch (DocumentClientException e)
//            {
//                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
//                {
//                    await client.CreateDocumentCollectionAsync(
//                        UriFactory.CreateDatabaseUri(this._Toolsettings["Values:DataBaseId"]),
//                        new DocumentCollection { Id = this._Toolsettings["Values:ContainerId"] });

//                }
//                else
//                {
//                    throw;
//                }
//            }
//        }
//    }
//}
