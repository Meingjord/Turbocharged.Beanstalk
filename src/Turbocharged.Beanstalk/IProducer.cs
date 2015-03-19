﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.Beanstalk
{
    public interface IProducer
    {
        /// <summary>
        /// Uses the specified tube. Jobs will be inserted into the currently-used tube.
        /// </summary>
        /// <returns></returns>
        Task<string> Use(string tube);

        /// <summary>
        /// Retrieves the name of the currently-used tube.
        /// </summary>
        Task<string> Using();

        /// <summary>
        /// Puts a new job into the currently-used tube.
        /// </summary>
        /// <param name="job">The job data.</param>
        /// <param name="priority">The priority of the job. Higher-priority jobs will be delivered before lower-priority jobs.</param>
        /// <param name="delay">The number of seconds the server should wait before allowing the job to be reserved.</param>
        /// <param name="ttr">The number of seconds for which this job will be reserved.</param>
        /// <returns>The ID of the inserted job.</returns>
        Task<int> PutAsync(byte[] job, int priority, int delay, int ttr);

        /// <summary>
        /// Retrives a job without reserving it.
        /// </summary>
        /// <returns>A job, or null if the job was not found.</returns>
        Task<JobDescription> PeekAsync(int id);

        /// <summary>
        /// Retrives the first job from the Ready state in the currently-used tube.
        /// </summary>
        /// <returns>A job, or null if there are no jobs in the Ready state.</returns>
        Task<JobDescription> PeekAsync();

        /// <summary>
        /// Retrives the first job from the specified state in the currently-used tube.
        /// </summary>
        /// <returns>A job, or null if no jobs are in the specified state.</returns>
        Task<JobDescription> PeekAsync(JobStatus status);
    }
}