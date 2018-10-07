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

## Facade Task

  > facade Task为操作或task提供一个可控和灵活的的facade task

  ```c#
  /*facade task实现*/
  TaskCompletionSource<T> existingOp = new TaskCompletionSource<T>();
  Task<T> facadeTask = existingOp.Task;
  
  Task.Factory.StartNew(() =>
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