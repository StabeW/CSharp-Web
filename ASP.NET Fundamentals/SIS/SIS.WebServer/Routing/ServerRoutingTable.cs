﻿using SIS.HTTP.Common;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Routing.Contracts;

namespace SIS.WebServer.Routing
{
    public class ServerRoutingTable : IServerRoutingTable
    {
        private readonly Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>> routes;
        public ServerRoutingTable()
        {
            routes = new Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>>
            {
                [HttpRequestMethod.Get] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Post] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Put] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Delete] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>()
            };
        }


        public void Add(HttpRequestMethod method, string path, Func<IHttpRequest, IHttpResponse> func)
        {
            CoreValidator.ThrowIfNull(method, nameof(method));
            CoreValidator.ThrowIfNullOrEmpty(path, nameof(path));
            CoreValidator.ThrowIfNull(func, nameof(func));

            routes[method].Add(path, func);
        }

        public bool Contains(HttpRequestMethod method, string path)
        {
            CoreValidator.ThrowIfNull(method, nameof(method));
            CoreValidator.ThrowIfNullOrEmpty(path, nameof(path));

            return routes.ContainsKey(method) && routes[method].ContainsKey(path);
        }

        public Func<IHttpRequest, IHttpResponse> Get(HttpRequestMethod method, string path)
        {
            CoreValidator.ThrowIfNull(method, nameof(method));
            CoreValidator.ThrowIfNullOrEmpty(path, nameof(path));

            return routes[method][path];
        }
    }
}
