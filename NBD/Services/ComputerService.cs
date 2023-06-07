using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using NBD.Models;

namespace NBD.Services
{
	public class ComputerService : IComputerService
	{
		private readonly IMongoCollection<ComputerModel> _computerCollection;
		private readonly GridFSBucket _gridFs;
		public ComputerService(
			IOptions<ComputerDatabaseSettings> computerDatabaseSettings, GridFSBucket gridFs)
		{
			var mongoClient = new MongoClient(
				computerDatabaseSettings.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(
				computerDatabaseSettings.Value.DatabaseName);

			_computerCollection = mongoDatabase.GetCollection<ComputerModel>(
				computerDatabaseSettings.Value.BooksCollectionName);

			_gridFs = gridFs;
		}

		public async Task<IEnumerable<ComputerModel>> GetComputers(int? year, string name)
		{
			var builder = new FilterDefinitionBuilder<ComputerModel>();
			var filter = builder.Empty;

			if (!string.IsNullOrWhiteSpace(name))
				filter = filter & builder.Regex("Name", new BsonRegularExpression(name));

			if (year.HasValue)
				filter = filter & builder.Eq("Year", year.Value);

			return await _computerCollection.Find(filter).ToListAsync();
		}

		public async Task<ComputerModel> GetComputer(string id) =>
			await _computerCollection.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();

		public async Task Create(ComputerModel c) => await _computerCollection.InsertOneAsync(c);

		public async Task Update(ComputerModel c) =>
			await _computerCollection.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(c.Id)), c);

		public async Task Remove(string id) => await _computerCollection.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
		public async Task<Byte[]> GetImage(string id) => await _gridFs.DownloadAsBytesAsync(new ObjectId(id));

		public async Task StoreImage(string id, Stream imageStream, string imageName)
		{
			var computers = await GetComputer(id);
			if (computers.HasImage())
				await _gridFs.DeleteAsync(new ObjectId(computers.ImageId));
			var imageId = await _gridFs.UploadFromStreamAsync(imageName, imageStream);
			computers.ImageId = imageId.ToString();

			var filter = Builders<ComputerModel>.Filter.Eq("_id", new ObjectId(computers.Id));
			var update = Builders<ComputerModel>.Update.Set("ImageId", computers.ImageId);

			await _computerCollection.UpdateOneAsync(filter, update);
		}
	}
}


