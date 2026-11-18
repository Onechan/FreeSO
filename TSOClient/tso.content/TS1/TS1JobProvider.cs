using System;
using FSO.Content.Framework;
using FSO.Files.Formats.IFF;
using FSO.Files.Formats.IFF.Chunks;
using System.Linq;

namespace FSO.Content.TS1
{
    public class TS1JobProvider
    {
        public IffFile JobResource;
        public int NumJobs;
        
        public TS1JobProvider(TS1Provider provider)
        {
            JobResource = (IffFile)provider.Get("work.iff");
            NumJobs = JobResource.List<CARR>().Count-1; //loads all jobs
        }

        public short GetJobData(ushort jobID, int jobLevel, int data)
        {
            return (short)(JobResource.Get<CARR>(jobID)?.GetJobData(jobLevel, data) ?? 0);
        }

        public CARR GetJob(ushort jobID)
        {
            return JobResource.Get<CARR>(jobID);
        }

        public short SetToNext(short current)
        {
            return (short)(JobResource.List<CARR>().FirstOrDefault(x => x.ChunkID > current)?.ChunkID ?? -1);
        }

        public string JobOffer(short jobID, int jobLevel)
        {
            //TODO: use STR#
            var job = JobResource.Get<CARR>((ushort)jobID);
            return (job?.Name ?? "(unknown)") + " career track for a " + job?.JobLevels[jobLevel].JobName + ".";
        }

        public STR JobStrings(short jobID)
        {
            //TODO: use STR#
            return JobResource.Get<STR>((ushort)jobID);
        }

        public JobLevel GetJobLevel(short jobID, int jobLevel)
        {
            var job = JobResource.Get<CARR>((ushort)jobID);
            if (job == null || job.JobLevels.Length == 0) return null;
            // Scripts can ask for a level past the career's last one (a child sent to military school in Sims 1);
            // use the nearest level instead of throwing.
            return job.JobLevels[Math.Clamp(jobLevel, 0, job.JobLevels.Length - 1)];
        }
    }
}
