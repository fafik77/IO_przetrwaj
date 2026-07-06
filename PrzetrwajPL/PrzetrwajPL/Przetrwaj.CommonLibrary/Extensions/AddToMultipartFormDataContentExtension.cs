namespace Przetrwaj.CommonLibrary.Extensions;

public interface IMultipartFormDataCreator
{
	public MultipartFormDataContent ToMultipartData(MultipartFormDataContent multipartFormData, string? rootPath);
}

public record KeyValue(string Key, object? Value) { }

public static class AddToMultipartFormDataContentExtension
{
	public static MultipartFormDataContent AddStringContent(this MultipartFormDataContent multipartFormData,
		KeyValue keyValue, string? rootKey = null)
	{
		if (keyValue.Value is null) return multipartFormData;
		var value = keyValue.Value.ToString()
			?? throw new ArgumentNullException($"{nameof(keyValue)}.{nameof(keyValue.Value)}");
		string fullKey = !string.IsNullOrEmpty(rootKey) ? $"{rootKey}.{keyValue.Key}" : keyValue.Key;
		multipartFormData.Add(new StringContent(value), name: fullKey);
		return multipartFormData;
	}

	public static MultipartFormDataContent AddContent<T>(this MultipartFormDataContent multipartFormData,
		string key, T? content, string? rootKey) where T : IMultipartFormDataCreator
	{
		if (content is null) return multipartFormData;
		string fullKey = !string.IsNullOrEmpty(rootKey) ? $"{rootKey}.{key}" : key;
		return content.ToMultipartData(multipartFormData, fullKey);
	}

	public static MultipartFormDataContent AddContent<T>(this MultipartFormDataContent multipartFormData,
		T content) where T : IMultipartFormDataCreator
	{
		return content.ToMultipartData(multipartFormData, null);
	}
}
