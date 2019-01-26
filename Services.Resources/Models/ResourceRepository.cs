using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Documents.Client;

namespace Services.Resources.Models {

    public class ResourceRepository : Repository<Resource> {

        public ResourceRepository(DocumentClient client, string databaseId, string collectionId) : base(client, databaseId, collectionId) {

            // Nothing to do here
        }
    }
}
