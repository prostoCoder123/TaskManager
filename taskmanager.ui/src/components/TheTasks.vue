<template>
  <div>
    <div v-if="loading">Loading tasks...</div>
    <div v-else-if="error">Error: {{ error }}</div>
    <div v-else>
      <h3> Total tasks: {{ total }} </h3>
      <table>
        <thead>
          <tr>
            <th v-for="(header, i) in [ 'Id', 'Title', 'Status', 'CreatedAt' ]"
                :key="`${header}${i}`"
                class="header-item">
              {{ header }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="task in tasks" class="table-rows">
            <td>{{ task.id }}</td>
            <td>{{ task.title }}</td>
            <td>{{ task.status }}</td>
            <td>{{ task.createdAt }}</td>
          </tr>
        </tbody>
      </table>
      <nav aria-label="Page navigation example">
        <div class="pagination">
          <span class="page-item">
            <button type="button" class="page-link" v-if="page != 0" @click="page--;fetchTasks()"> Previous </button>
          </span>
          <span class="page-item">
            <button type="button"
                    class="page-link"
                    v-for="pageNumber in Array.from({ length: pageCount+1 }, (value, index) => index)"
                    @click="page = pageNumber;fetchTasks()">
            {{pageNumber}}
            </button>
          </span>
          <span class="page-item">
            <button type="button" @click="page++;fetchTasks()" v-if="page < pageCount" class="page-link"> Next </button>
          </span>
        </div>
      </nav>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';

  const tasks = ref([]);
  const loading = ref(true);
  const error = ref(null);

  const total = ref(0);
  const page = ref(0);
  const perPage = ref(3);
  const pageCount = ref(0);

  const fetchTasks = async () =>
  {
    try
    {
      const response = await fetch(`api?page=${page.value}&count=${perPage.value}`, { mode: 'cors' });
      if (!response.ok)
      {
        throw new Error(`HTTP error, status: ${response.status}`);
      }

      const data = await response.json();
      //console.log(data);
      tasks.value = data.tasks;
      total.value = data.total;
      pageCount.value = Math.floor(data.total / perPage.value);
    }
    catch (err)
    {
      error.value = err.message;
    }
    finally
    {
      loading.value = false;
    }
  };

  onMounted(() => {
    fetchTasks();
  });
</script>

<style>
  .pagination {
    display: inline-block;
  }
</style>
