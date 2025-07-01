# TaskManager

This pet-project can perform several operations on project tasks:

- store tasks data in postgres db
- retrieve tasks data page by page filtered by status or all at once (paging & filtering are configurable)
- create new tasks
- modify tasks data
- validate tasks data


## Entities

A project task is an entity that has following properties:

- Id (unique autoincremented value)
- Title
- Description
- DueDate
- Status
- CreatedAt
- UpdatedAt
- CompletedAt

## Run the app

To launch this app first of all make sure that Docker is running on your machine.

Select the TaskManager.AppHost project as startup project and the 'https' profile and then click Run (F5).

The panel with the distributed app will be opened in your web-browser, where you can see all the docker containers that will start in the defined order.
Wait until the taskmanager container will be in the 'Running' state and then click on the taskmanager url such as:
https://localhost:7096

## WEB API

To retrieve tasks enter the url https://localhost:7096/tasks in the browser address bar.
This GET method returns the first page with the existing tasks sorted by CreatedAt in descending order (the default count of tasks per page is 3).

You can configure page and filter settings by adding url query parameters as shown below:

https://localhost:7096/tasks?count=5&page=0&status=new

https://localhost:7096/tasks?count=2&page=1&status=inprogress

To create a task send POST request to https://localhost:7096/tasks with the body as shown below:
```
{
    "title": "new task 3",
    "description": "some description",
    "dueDate": "2025-06-30T19:25:43.511Z"
}
```
Note that you can not create two tasks with the same titles.

Make sure you specified the correct timezone for timestamps:

- "2025-06-30T19:25:43.511Z" belongs to UTC timezone (+00:00),
- "2025-06-30T19:25:43.511+03:00" belongs to 'Europe/Moscow' timezone, etc.

To update some properties of a task send PATCH request to https://localhost:7096/tasks with the body as shown below:
```
{
    "id": 5,
    "title": "task to implement"
}
```
OR
```
{
    "id": 5,
    "status": 1,
    "description": "new description"
}
```
etc.

Task statuses are mapped in the following way:

- 0 = New
- 1 = InProgress
- 2 = Completed
- 3 = OverDue



