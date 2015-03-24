﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    class KickRequest : Request<int>
    {
        public Task<int> Task { get { return _tcs.Task; } }
        int _count;

        TaskCompletionSource<int> _tcs = new TaskCompletionSource<int>();

        public KickRequest(int count)
        {
            _count = count;
        }

        public byte[] ToByteArray()
        {
            return "kick {0}\r\n".FormatWith(_count).ToASCIIByteArray();
        }

        public void Process(string firstLine, NetworkStream stream)
        {
            var parts = firstLine.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                if (parts.Length == 2 && parts[0] == "KICKED")
                {
                    var kicked = Convert.ToInt32(parts[1]);
                    _tcs.SetResult(kicked);
                }
            }
            catch
            {
            }
            _tcs.SetException(new Exception("Unknown response from kick: '{0}'".FormatWith(firstLine)));
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}