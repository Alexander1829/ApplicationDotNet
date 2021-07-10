using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace mylabel.MobileId.Aggregator.BusinessLogic
{
	public class CacheAccessor
	{
		IMemoryCache memoryCache;

		public CacheAccessor(IMemoryCache memoryCache) => this.memoryCache = memoryCache;

		public Task<T> GetOrCreateAsync<T>(string key, int expirationInSeconds, Func<Task<T>> workAsync) => memoryCache.GetOrCreateAsync(key, e =>
		{
			e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expirationInSeconds);

			return workAsync();
		});

		public bool TryGetValue<T>(string key, out T value)
		{
			return memoryCache.TryGetValue(key, out value);
		}

		public T Get<T>(string key) //where : T notnull
		{
			return memoryCache.Get<T>(key);
		}

		public void RemoveFromCache(string key)
		{
			memoryCache.Remove(key);
		}


		public TItem Set<TItem>(object key, TItem value, MemoryCacheEntryOptions options)
		{
			return memoryCache.Set(key, value, options);
		}
	}
}