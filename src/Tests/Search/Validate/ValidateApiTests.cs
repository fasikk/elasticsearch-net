﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Tests.Framework;
using Tests.Framework.Integration;
using Xunit;
using Tests.Framework.MockData;

namespace Tests.Search.Validate
{
	[Collection(IntegrationContext.ReadOnly)]
	public class ValidateApiTests 
		: ApiTestBase<IValidateResponse, IValidateQueryRequest, ValidateQueryDescriptor<Project>, ValidateQueryRequest<Project>>
	{
		public ValidateApiTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override bool ExpectIsValid => true;

		protected override int ExpectStatusCode => 200;

		protected override HttpMethod HttpMethod => HttpMethod.POST;

		protected override string UrlPath => "/project/project/_validate/query";

		protected override LazyResponses ClientUsage() => Calls(
			fluent: (c, f) => c.Validate<Project>(v => v.Query(q => q.MatchAll())),
			fluentAsync: (c, f) => c.ValidateAsync<Project>(v => v.Query(q => q.MatchAll())),
			request: (c, r) => c.Validate(new ValidateQueryRequest<Project> { Query = new QueryContainer(new MatchAllQuery()) }),
			requestAsync: (c, r) => c.ValidateAsync(new ValidateQueryRequest<Project> { Query = new QueryContainer(new MatchAllQuery()) })
		);
	}

	[Collection(IntegrationContext.ReadOnly)]
	public class ValidateInvalidQueryApiTests 
		: ApiTestBase<IValidateResponse, IValidateQueryRequest, ValidateQueryDescriptor<Project>, ValidateQueryRequest<Project>>
	{
		public ValidateInvalidQueryApiTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override bool ExpectIsValid => true;

		protected override int ExpectStatusCode => 200;

		protected override HttpMethod HttpMethod => HttpMethod.POST;

		protected override string UrlPath => "/project/project/_validate/query";

		private ValidateQueryDescriptor<Project> _descriptor = new ValidateQueryDescriptor<Project>()
			.Query(q => q
				.Match(m => m
					.OnField(p => p.StartedOn)
					.Query("shouldbeadate")
				)
			);

		private ValidateQueryRequest<Project> _request = new ValidateQueryRequest<Project>
		{
			Query = new QueryContainer(
				new MatchQuery
				{
					Field = "startedOn",
					Query = "shouldbeadate"
				}
			)
        };

		protected override LazyResponses ClientUsage() => Calls(
			fluent: (c, f) => c.Validate<Project>(v => _descriptor),
			fluentAsync: (c, f) => c.ValidateAsync<Project>(v => _descriptor),
			request: (c, r) => c.Validate(_request),
			requestAsync: (c, r) => c.ValidateAsync(_request)
		);

		[I]
		public async Task IsInvalid() => await this.AssertOnAllResponses(r => r.Valid.Should().BeFalse());
	}
}
