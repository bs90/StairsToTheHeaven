using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

// A proxy object acting as a launcher for coroutines
public class BackupJobManager : MonoSingleton<JobManager> 
{

}

public class Job
{
	public event Action<bool> onComplete;


	private bool running;
	public bool isRuning {
		get {
			return running;
		}
	}

	private bool paused;
	public bool isPaused {
		get {
			return paused;
		}
	}

	private IEnumerator currentCoroutine;
	private bool jobWasKilled;
	private Stack<Job> childJobStack;
	private Stack<Job> jobsStack;

	#region Constructors

	public Job(IEnumerator coroutine) : this(coroutine, 0, true)
	{

	}

	public Job(IEnumerator coroutine, float delay) : this(coroutine, delay, true)
	{
		currentCoroutine = coroutine;
		StartJob(delay);
	}

	public Job(IEnumerator coroutine, float delay, bool shouldStart)
	{
		currentCoroutine = coroutine;
		if (shouldStart) {
			StartJob(delay);
		}
	}

	#endregion

	#region Static Make

	public static Job Create(IEnumerator coroutine)
	{
		return new Job(coroutine);
	}

	public static Job Create(IEnumerator coroutine, float delay)
	{
		return new Job(coroutine, delay);
	}

	public static Job Create(IEnumerator coroutine, float delay, bool shouldStart)
	{
		return new Job(coroutine, delay, shouldStart);
	}

	#endregion

	#region Public API

	public Job CreateAndAddChildJob(IEnumerator coroutine, float delay)
	{
		Job job = new Job(coroutine, delay, false);
		AddChildJob(job);
		return job;

	}

	public void AddJob(Job job)
	{
		if (jobsStack == null) {
			jobsStack = new Stack<Job>();
		}
		jobsStack.Push(job);
	}

	public void AddChildJob(Job childJob)
	{
		if (childJobStack == null) {
			childJobStack = new Stack<Job>();
		}
		childJobStack.Push(childJob);
	}

	public void RemoveJob(Job job)
	{
		if (jobsStack.Contains(job)) {
			Stack<Job> stack = new Stack<Job>(jobsStack.Count - 1);
			Job[] allCurrentJob = jobsStack.ToArray();
			Array.Reverse(allCurrentJob);

			for(int i = 0; i < allCurrentJob.Length; i++) {
				Job currentJob = allCurrentJob[i];
				if (currentJob != job) {
					stack.Push(currentJob);
				}
			}
			jobsStack = stack;
		}
	}

	public void RemoveChildJob(Job childJob)
	{
		if (childJobStack.Contains(childJob)) {
			Stack<Job> childStack = new Stack<Job>(childJobStack.Count - 1);
			Job[] allCurrentChildren = childJobStack.ToArray();
			Array.Reverse(allCurrentChildren);

			for (int i = 0; i < allCurrentChildren.Length; i++) {
				Job job = allCurrentChildren[i];
				if (job != childJob) {
					childStack.Push(job);
				}
			}
			childJobStack = childStack;
		}
	}

	public void RemoveAllJobs()
	{
		if (jobsStack.Count > 0) {
			jobsStack.Clear();
		}
	}

	public void RemoveAllChildJob()
	{
		if (childJobStack.Count > 0) {
			childJobStack.Clear();
		}
	}

	public void StartJob(float delay)
	{
		if (currentCoroutine == null) {
			Debug.LogError("No job to start");
			return;
		}
//		if (jobCompleted != null && childJobStack == null) {
//			Debug.LogError("Job already completed");
//			return;
//		}
		running = true;
		JobManager.Instance.StartCoroutine(RunJob(delay));
	}

	public IEnumerator StartJobAsCoroutine(float delay)
	{
		running = true;
		yield return JobManager.Instance.StartCoroutine(RunJob(delay));
	}

	public void Pause()
	{
		paused = true;
	}

	public void Unpause()
	{
		paused = false;
	}

	public void KillJob()
	{
		currentCoroutine = null;
		jobWasKilled = true;
		running = false;
		paused = false;
	}

	public void KillJob(float delayInSeconds)
	{
		int delay = (int)(delayInSeconds * 1000);
		new Timer(obj => {
			lock(this) {
				KillJob();
			}
		}, null, delay, Timeout.Infinite);

	}

	#endregion

	private IEnumerator RunJob(float delay)
	{
		yield return new WaitForSeconds(delay);
		yield return null;

		while (running) {
			if (paused) {
				yield return null;		
			}
			else {
				if (currentCoroutine.MoveNext()) {
					yield return currentCoroutine.Current;
				}
				else {
					if (childJobStack != null) {
						Debug.Log ("Run a child");
						yield return JobManager.Instance.StartCoroutine(RunChildJobs(delay));
					}
					running = false;
				}
			}
		}
		if (onComplete != null) {
			onComplete(jobWasKilled);
		}
		if (jobsStack != null) {
			Debug.Log ("Run a next job" + jobsStack.ToString());
			yield return JobManager.Instance.StartCoroutine(RunNextJobs(delay));
		}
	}

	private IEnumerator RunNextJobs(float delay) {
		if (jobsStack != null && jobsStack.Count > 0) {
			Stack<Job> reverseStack = new Stack<Job>(jobsStack.ToArray());
			RemoveAllJobs();
			do {
				Job job = reverseStack.Pop();
				yield return JobManager.Instance.StartCoroutine(job.StartJobAsCoroutine(delay));
			}
			while (reverseStack.Count > 0);
		}
	}

	private IEnumerator RunChildJobs(float delay)
	{
		if (childJobStack != null && childJobStack.Count > 0) {
			Stack<Job> reverseStack = new Stack<Job>(childJobStack.ToArray());
			RemoveAllChildJob();
			do {
				Job childJob = reverseStack.Pop();
				yield return JobManager.Instance.StartCoroutine(childJob.StartJobAsCoroutine(delay));
			}
			while (reverseStack.Count > 0);
		}
	}
}
