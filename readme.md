# .Net Async

## Task UI Refresh

 * Winform和WPF仅允许一个UI线程，其他线程需要更新UI时，需要切换Context
 * Task.ContinueWith 该Task对象执行完事之后执行

  ```c#
  btn.Content = "Runing";
  
  //Work Task
  Task workTask = new Task(
      () => {
      Thread.Sleep(1000);
  });
  
  //Update Task
  Task updateTask = workTask.ContinueWith(
      (argc) =>
      {
          btn.Content = "Run";
      }, 
  TaskScheduler.FromCurrentSynchronizationContext());
  
  //Start Task
  workTask.Start();
  ```

* Task.Factory.StartNew() 创建并开始Task
    * StartNew [doesn't understand asynchronous delegates](http://blog.stephencleary.com/2013/08/startnew-is-dangerous.html)
    * StartNew是一个低层次的API，应该使用Task.Run来替代

* async/await
    * async void [does not allow the calling code to know when it completes](https://msdn.microsoft.com/en-us/magazine/jj991977.aspx)

## Facade Task

  > facade Task为操作或task提供一个可控和灵活的的facade task

  ```c#
  /*facade task实现*/
  TaskCompletionSource<T> existingOp = new TaskCompletionSource<T>();
  Task<T> facadeTask = existingOp.Task;
  
  //Task.Factory.StartNew(() =>
  Task.Run(() =>
  {
      //执行操作或task
      ...
      //控制facade的执行结果
      existingOp.SetResult(t);
      //Or
      existingOp.SetCanceled();
  });
  
  T result = t1.Result;
  
  //----------------------------------------------------//
  /*TaskCompletionSource 提供操作*/
  public class TaskCompletionSource<TResult>
  {
      public TaskCompletionSource();
      public TaskCompletionSource(object state);
      public TaskCompletionSource(TaskCreationOptions creationOptions);
      public TaskCompletionSource(object state, TaskCreationOptions creationOptions);
  
      public Task<TResult> Task { get; }
  
      public void SetCanceled();
      public void SetException(Exception exception);
      public void SetException(IEnumerable<Exception> exceptions);
      public void SetResult(TResult result);
      public bool TrySetCanceled();
      public bool TrySetException(Exception exception);
      public bool TrySetException(IEnumerable<Exception> exceptions);
      public bool TrySetResult(TResult result);
  }
  ```

* 控制Task执行结果
  ```c#
  Task.Run(async() =>
    {
        int value = (new Random()).Next(100);
        Console.WriteLine("Value: " + value);

        //控制facade的执行结果
        if (value < 33)
            await Task.FromException(new Exception("Less than 33"));
        else if (value < 66)
        {
            CancellationTokenSource TokenSource = new CancellationTokenSource();
            TokenSource.Cancel();

            await Task.FromCanceled(TokenSource.Token);
        }
  });
  ```

## Task Knowledge

* Task Technology

![](https://github.com/xiong-ang/.Net-Task/blob/master/Imgs/Async%20Technology%20Layer.PNG?raw=true)

  * 等待Task执行完：task.Wait()

  * task.Result 会执行隐式等待

  * Task.WaitAll/WaitAny(taskArray) 等待tasks

  * WaitAllOneByOne

    ![](https://github.com/xiong-ang/.Net-Task/blob/master/Imgs/WaitAllOneByOne.PNG?raw=true)

* Task组合

![](https://github.com/xiong-ang/.Net-Task/blob/master/Imgs/TaskComposition.PNG?raw=true)

* Task.ContinueWhenAll/Task.ContinueWhenAny 多对一的Task组合

* Task Exception

  * 如果Task里抛出未处理的异常，将会导致：1.Task终止；2.异常被catch，保持在task的Exception属性总；3.异常将作为AggregateException的一部分，re-throw到.Wait/Result/WaitAll

  * 异常处理

    ![](https://github.com/xiong-ang/.Net-Task/blob/master/Imgs/TaskExceptionHandler.PNG?raw=true)

* Task Cancellation

  ![](https://github.com/xiong-ang/.Net-Task/blob/master/Imgs/TaskCancellation.PNG?raw=true)

  * Task Creator Job

    ![](https://github.com/xiong-ang/.Net-Task/blob/master/Imgs/TaskCreatorJob.PNG?raw=true)

  * Cancellation Task Job

    ![](https://github.com/xiong-ang/.Net-Task/blob/master/Imgs/CanceledTaskJob.PNG?raw=true)

* Pass Data

  * Child-Parent Task

    ![](https://github.com/xiong-ang/.Net-Task/blob/master/Imgs/Child-Parent%20Task.PNG?raw=true)

