﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Turbocharged.Beanstalk
{
    public sealed class BeanstalkConnection : IProducer, IConsumer, IDisposable
    {
        string _hostname;
        int _port;
        PhysicalConnection _connection;

        public BeanstalkConnection(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
        }

        public void Connect()
        {
            _connection = new PhysicalConnection(_hostname, _port);
            _connection.Connect();
        }

        public void Close()
        {
            try
            {
                _connection.Close();
            }
            finally
            {
                _connection = null;
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public IProducer AsProducer()
        {
            return this;
        }

        public IConsumer AsConsumer()
        {
            return this;
        }

        #region Producer

        Task<string> IProducer.Use(string tube)
        {
            var request = new UseRequest(tube);
            return SendAndGetResult(request);
        }

        Task<string> IProducer.Using()
        {
            var request = new UsingRequest();
            return SendAndGetResult(request);
        }

        Task<int> IProducer.PutAsync(byte[] job, int priority, int delay, int ttr)
        {
            var request = new PutRequest
            {
                Priority = priority,
                Delay = delay,
                TimeToRun = ttr,
                Job = job,
            };
            return SendAndGetResult(request);
        }

        Task<JobDescription> IProducer.PeekAsync()
        {
            return ((IProducer)this).PeekAsync(JobStatus.Ready);
        }

        Task<JobDescription> IProducer.PeekAsync(JobStatus status)
        {
            var request = new PeekRequest(status);
            return SendAndGetResult(request);
        }

        Task<JobDescription> IProducer.PeekAsync(int id)
        {
            var request = new PeekRequest(id);
            return SendAndGetResult(request);
        }

        #endregion

        #region Consumer

        Task<int> IConsumer.Watch(string tube)
        {
            var request = new WatchRequest(tube);
            return SendAndGetResult(request);
        }

        Task<int> IConsumer.Ignore(string tube)
        {
            var request = new IgnoreRequest(tube);
            return SendAndGetResult(request);
        }

        Task<List<string>> IConsumer.Watched()
        {
            var request = new WatchedRequest();
            return SendAndGetResult(request);
        }

        Task<JobDescription> IConsumer.ReserveAsync(TimeSpan timeout)
        {
            var request = new ReserveRequest(timeout);
            return SendAndGetResult(request);
        }

        Task<JobDescription> IConsumer.ReserveAsync()
        {
            var request = new ReserveRequest();
            return SendAndGetResult(request);
        }

        Task<JobDescription> IConsumer.PeekAsync(int id)
        {
            return ((IProducer)this).PeekAsync(id);
        }

        #endregion

        async Task<T> SendAndGetResult<T>(Request<T> request)
        {
            await _connection.SendAsync(request).ConfigureAwait(false);
            return await request.Task; // Let the consumer decide whether to ConfigureAwait or not
        }
    }
}