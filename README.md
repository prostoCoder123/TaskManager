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
- CreatedAt
- UpdatedAt
- CompletedAt

## Run the app

To launch this app first of all make sure that Docker is running on your machine.

Select the TaskManager.AppHost project as startup project and the 'https' profile and then click on Run button.

The app page will be opened in your web-browser, where you can see all the docker containers that will start in the defined order.
Wait until the taskmanager container will be in the 'Running' state and then click on the app link such as:
https://localhost:7096

## WEB API

To retrieve tasks enter the url https://localhost:7096/tasks in the browser address bar.
This GET method returns the first page with tasks ordered by CreatedAt prop (the default count of tasks on page = 3).

You can configure page and filter settings by adding url query parameters as shown below:
https://localhost:7096/tasks?count=5&page=0&status=new
https://localhost:7096/tasks?count=2&page=1&status=inprogress

To create Task send POST request to https://localhost:7096/tasks with the body as shown below:
```
{
    "title": "new task 3",
    "description": "some description",
    "dueDate": "2025-06-30T19:25:43.511Z"
}
```
To update some properties of the task send PATCH request to https://localhost:7096/tasks with the body as shown below:
```
{
    "id": 5,
    "title": "task to implement"
}
```




