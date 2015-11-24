using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

// A proxy object acting as a launcher for coroutines
public class JobManager : MonoSingleton<JobManager> 
{

}

public class Job
{
	public event Action<bool> jobCompleted;

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

	#region Constructors

	public Job(IEnumerator coroutine) : this(coroutine, true)
	{

	}

	public Job(IEnumerator coroutine, bool shouldStart)
	{
		currentCoroutine = coroutine;
		if (shouldStart) {
			StartJob();
		}
	}

	public Job(IEnumerator coroutine, float delay) : this(coroutine, true)
	{
		currentCoroutine = coroutine;
	}

	#endregion

	#region Static Make

	public static Job Create(IEnumerator coroutine)
	{
		return new Job(coroutine);
	}

	public static Job Create(IEnumerator coroutine, bool shouldStart)
	{
		return new Job(coroutine, shouldStart);
	}

	public static Job Create(IEnumerator coroutine, float delay)
	{
		return new Job(coroutine, delay);
	}

	#endregion

	#region Public API

	public Job CreateAndAddChildJob(IEnumerator coroutine)
	{
		Job job = new Job(coroutine, false);
		AddChildJob(job);
		return job;

	}

	public void AddChildJob(Job childJob)
	{
		if (childJobStack == null) {
			childJobStack = new Stack<Job>();
		}
		childJobStack.Push(childJob);
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

	public void RemoveAllChildJob()
	{
		if (childJobStack.Count > 0) {
			childJobStack.Clear();
		}
	}

	public void StartJob()
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
		JobManager.Instance.StartCoroutine(RunJob());
	}

	public void StartJobWithDelay(float delay)
	{
		if (currentCoroutine == null) {
			Debug.LogError("No job to start");
			return;
		}
		running = true;
		JobManager.Instance.StartCoroutine(RunJob());
	}

	public IEnumerator StartJobAsCoroutine()
	{
		running = true;
		yield return JobManager.Instance.StartCoroutine(RunJob());
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

	private IEnumerator RunJob()
	{
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
						yield return JobManager.Instance.StartCoroutine(RunChildJobs());
					}
					running = false;
				}
			}
		}
		if (jobCompleted != null) {
			jobCompleted(jobWasKilled);
		}
	}

	private IEnumerator RunChildJobs()
	{
		if (childJobStack != null && childJobStack.Count > 0) {
			Stack<Job> reverseStack = new Stack<Job>(childJobStack.ToArray());
			RemoveAllChildJob();
			do {
				Job childJob = reverseStack.Pop();
				yield return JobManager.Instance.StartCoroutine(childJob.StartJobAsCoroutine());
			}
			while (reverseStack.Count > 0);
		}
	}
}
