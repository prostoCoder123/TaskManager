<template>
  <div>
    <div v-if="loading">Loading tasks...</div>
    <div v-else-if="error">Error: {{ error }}</div>
    <div v-else>
      <h3> Total tasks: {{ total }} </h3>
      <v-data-table-server v-model:items-per-page="perPage"
                           :items-per-page-options="[3,5]"
                           v-model:page="page"
                           :items="tasks"
                           :items-length="total"
                           :loading="loading"
                           :search="search"
                           item-value="name"
                           @update:options="fetchTasks">
        <template v-slot:tfoot>
          <tr>
            <td></td>
            <td></td>
            <td></td>
            <td rowspan="3">
              <v-autocomplete label="Autocomplete"
                              :items="statuses"
                              v-model="search"
                              v-bind="selected"></v-autocomplete>
            </td>
          </tr>
        </template>
      </v-data-table-server>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, watch } from 'vue';

  const tasks = ref([]);
  const loading = ref(true);
  const error = ref(null);
  const statuses = ['Новая', 'В процессе', 'Завершена', 'Просрочена']

  const total = ref(0);
  const page = ref(1);
  const perPage = ref(3);
  const search = ref(null)
  const selected = ref('')

  const fetchTasks = async () =>
  {
    console.log(search.value);
    try
    {
      const response = await fetch(
        `api?page=${page.value - 1}&count=${perPage.value}` +
        (search.value == null
          ? ""
          : `&status=${statuses.indexOf(search.value)}`), { mode: 'cors' });

      if (!response.ok)
      {
        throw new Error(`HTTP error, status: ${response.status}`);
      }

      const data = await response.json();
      //console.log(data);
      tasks.value = data.tasks;
      total.value = data.total;
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

  watch(selected, () => {
    search.value = selected;
    fetchTasks();
    console.log("selected " + selected);
  })
</script>
