﻿using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Constants;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Models.Enums;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;

namespace Ecoeden.Search.Api.Services.Search;

public class SearchService<TDocument>(ILogger logger, IOptions<ElasticSearchOption> options) 
    : SearchBaseService(logger, options), ISearchService<TDocument> 
    where TDocument : class
{
    public async Task<Result<bool>> SeedDocumentAsync(TDocument document, string id, string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Requesting - storing data to elastic search {index}", index);

        if(!await IndexExist(index))
        {
            await CreateNewIndex<TDocument>(index);
        }

        var indexResponse = await ElasticsearchClient.IndexAsync(document, idx => idx.Index(index)
            .Id(id)
            .Refresh(Refresh.WaitFor));

        if (!indexResponse.IsValid)
        {
            _logger.Here().Error("Data seeding to elastic serach index {@index} failed", index);
            return Result<bool>.Failure(ErrorCodes.OperationFailed, ErrorMessages.Operationfailed);
        }

        _logger.Here().Information("Data seeding completed successfully for document {id}", id);
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);

    }
    
    public async Task<Result<bool>> UpdateDocumentAsync(TDocument document, Dictionary<string, string> fieldValue, string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - update document in elastic search {@index}", index);

        var documentResponse = await ElasticsearchClient
                .SearchAsync<TDocument>(s => s.Index(index)
                .Query(q => q.Match(m => m
                    .Field(fieldValue.Keys.First())
                    .Query(fieldValue.Values.First())
                )));

        var docId = documentResponse.Hits.First().Id;
        var data = documentResponse.Documents.First();
        var documentUpdateResponse = await ElasticsearchClient.UpdateAsync<TDocument, object>(
                new DocumentPath<TDocument>(docId),
                u => u.Doc(document)
                    .DocAsUpsert()
            );
        var docData = documentUpdateResponse.Result;

        if (!documentUpdateResponse.IsValid)
        {
            _logger.Here().Error("Elastic search docuemnt update failed");
            return Result<bool>.Failure(ErrorCodes.OperationFailed, "Elastic document update failed");
        }

        _logger.Here().Information("Elastic document update successfull");
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SearchReIndex(IEnumerable<TDocument> documents, string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - reindex search data for {@index}", index);

        if (await IndexExist(index))
        {
            var deleteResposne = await ElasticsearchClient.Indices.DeleteAsync(index);
            if (!deleteResposne.IsValid)
            {
                _logger.Here().Error("Index deletion failed");
                return Result<bool>.Failure(ErrorCodes.OperationFailed, "Index deletion failed");
            }
        }

        await CreateNewIndex<TDocument>(index);

        var bulkResponse = await ElasticsearchClient.BulkAsync(b => b.Index(index).IndexMany(documents));

        if (!bulkResponse.IsValid)
        {
            _logger.Here().Error("Re-indexing failed");
            return Result<bool>.Failure(ErrorCodes.OperationFailed, "Re-index operation failed");
        }

        _logger.Here().Information("Re-index for {index} successful", index);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveDocumentAsync(Dictionary<string, object> query, string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - remove data from elastic search {index}", index);

        if (!await IndexExist(index))
        {
            return Result<bool>.Failure(ErrorCodes.OperationFailed, $"Elastic operation failed, index {index} was not found");
        }

        var fieldName = query.Keys.First();
        var fieldValue = query.Values.First();

        var deleteResponse = await ElasticsearchClient.DeleteByQueryAsync<TDocument>(d => d
                .Index(index)
                .Query(q => q
                .Match(m => m.Field(fieldName).Query(fieldValue.ToString()))));

        if (!deleteResponse.IsValid)
        {
            _logger.Here().Error("Elastic search delete operation failed");
            return Result<bool>.Failure(ErrorCodes.OperationFailed, "Elastic document delete operation failed");
        }

        _logger.Here().Information("Elastic document delete successful");
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }
}