﻿using Catalog.Core.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Application.Responses
{
    public class ProductResponse
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string ImageFile { get; set; }

        public ProductBrand Brands { get; set; }

        public ProductType Types { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
        public decimal Price { get; set; }
    }
}
