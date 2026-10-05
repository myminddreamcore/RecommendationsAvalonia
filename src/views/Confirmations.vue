<script setup>
import axios from 'axios';
import { ref, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const token = ref('');
const userId = ref('');

const incomingRequests = ref([]);
const outgoingRequests = ref([]);
const filterStatus = ref('all');
const showDetailsModal = ref(false);
const selectedRequest = ref(null);
let intervalId = null;

const getToken = () => {
  const encrypted = localStorage.getItem('auth_token');
  if (!encrypted) return null;
  return atob(encrypted);
};

const formatDate = (dateString) => {
  if (!dateString) return 'Дата не указана';
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return dateString;
  const day = String(date.getDate()).padStart(2, '0');
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const year = date.getFullYear();
  return `${day}.${month}.${year}`;
};

const loadConfirmations = async () => {
  token.value = getToken();
  userId.value = localStorage.getItem('user_id');
  
  try {
    const incomingResp = await axios.get(`http://localhost:5015/User/users/${userId.value}/confirmations/incoming`);
    incomingRequests.value = incomingResp.data || [];
    
    const outgoingResp = await axios.get(`http://localhost:5015/User/users/${userId.value}/confirmations/outgoing`);
    outgoingRequests.value = outgoingResp.data || [];
  } catch (err) {
    console.error(err);
  }
};

const confirmRequest = async (req) => {
  try {
    await axios.put(`http://localhost:5015/Confirmation/confirmations/request/${req.c.idConfirmation}/accept`, {});
    await loadConfirmations();
  } catch (err) {
    alert('Ошибка подтверждения');
  }
};

const rejectRequest = async (req) => {
  try {
    await axios.put(`http://localhost:5015/Confirmation/confirmations/request/${req.c.idConfirmation}/reject`, {});
    await loadConfirmations();
  } catch (err) {
    alert('Ошибка отклонения');
  }
};

const revokeConfirmation = async (req) => {
  if (confirm('Вы уверены, что хотите отозвать подтверждение? Это действие необратимо.')) {
    try {
      await axios.put(`http://localhost:5015/Confirmation/confirmations/request/${req.c.idConfirmation}/ot`, {});
      await loadConfirmations();
    } catch (err) {
      alert('Ошибка отзыва');
    }
  }
};

const filteredIncoming = () => {
  if (filterStatus.value === 'all') return incomingRequests.value;
  if (filterStatus.value === 'null') {
    return incomingRequests.value.filter(r => r.c?.status === null);
  }
  return incomingRequests.value.filter(r => r.c?.status === filterStatus.value);
};

const showDetails = (req) => {
  selectedRequest.value = req;
  showDetailsModal.value = true;
};

onMounted(() => {
  loadConfirmations();
  intervalId = setInterval(() => {
    loadConfirmations();
  }, 30000);
});

onUnmounted(() => {
  if (intervalId) clearInterval(intervalId);
});
</script>

<template>
  <div class="confirmations-container">
    <div class="back">
      <button @click="router.push('/dashboard')" class="back-btn">← Назад</button>
    </div>

    <h1>Управление подтверждениями</h1>

    <div class="filters">
      <button @click="filterStatus='all'" :class="{active: filterStatus==='all'}">Все</button>
      <button @click="filterStatus='null'" :class="{active: filterStatus==='null'}">Ожидают</button>
      <button @click="filterStatus='Подтверждено'" :class="{active: filterStatus==='Подтверждено'}">Подтверждены</button>
    </div>

    <div class="section">
      <h2>Входящие запросы</h2>
      <div v-if="filteredIncoming().length === 0" class="empty">
        У вас нет активных запросов на подтверждение
      </div>

      <div v-for="req in filteredIncoming()" :key="req.idConfirmation" class="card">
        <div class="card-content">
          <p class="request-text">
            <strong>{{ req.nameUser }}</strong> 
            запросил подтверждение <strong>{{ req.nameSkill }}</strong> 
            ({{ formatDate(req.c?.date) }})
          </p>

          <div v-if="req.c?.status === null" class="actions">
            <button @click="confirmRequest(req)" class="btn-confirm">Подтвердить</button>
            <button @click="rejectRequest(req)" class="btn-reject">Отклонить</button>
            <button @click="showDetails(req)" class="btn-details">Просмотр деталей</button>
          </div>

          <div v-else-if="req.c?.status === 'Подтверждено'" class="status status-confirmed">
            Подтверждено
          </div>
          <div v-else-if="req.c?.status === 'Отклонено'" class="status status-rejected">
            Отклонено
          </div>
        </div>
      </div>
    </div>

    <div class="section">
      <h2>Исходящие запросы</h2>
      <div v-if="outgoingRequests.length === 0" class="empty">
        У вас нет исходящих запросов
      </div>

      <div v-for="req in outgoingRequests" :key="req.idConfirmation" class="card">
        <div class="card-content">
          <p class="request-text">
            Запрос для <strong>{{ req.nameUser }}</strong>
            на навык <strong>{{ req.nameSkill }}</strong>
          </p>
          <p class="date">{{ formatDate(req.c?.date) }}</p>

          <div v-if="req.c?.status === null" class="status status-pending">
            Ожидает
          </div>
          <div v-else-if="req.c?.status === 'Подтверждено'" class="status-with-action">
            <span class="status status-confirmed">Подтверждено</span>
            <button @click="revokeConfirmation(req)" class="btn-revoke">Отозвать</button>
          </div>
          <div v-else-if="req.c?.status === 'Отклонено'" class="status status-rejected">
            Отклонено
          </div>
        </div>
      </div>
    </div>

    <div v-if="showDetailsModal" class="modal" @click.self="showDetailsModal=false">
      <div class="modal-content">
        <h3>Детали запроса</h3>
        <div class="modal-row">
          <span class="modal-label">Навык</span>
          <span class="modal-value">{{ selectedRequest?.nameSkill }}</span>
        </div>
        <div class="modal-row">
          <span class="modal-label">От кого</span>
          <span class="modal-value">{{ selectedRequest?.nameUser }}</span>
        </div>
        <div class="modal-row">
          <span class="modal-label">Дата</span>
          <span class="modal-value">{{ formatDate(selectedRequest?.c?.date) }}</span>
        </div>
        <div class="modal-row">
          <span class="modal-label">Статус</span>
          <span class="modal-value">{{ selectedRequest?.c?.status || 'Ожидает' }}</span>
        </div>
        <button @click="showDetailsModal=false" class="modal-close-btn">Закрыть</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.confirmations-container {
  max-width: 900px;
  margin: 0 auto;
  padding: 32px 28px;
  background: #faf8f5;
  min-height: 100vh;
}

.back {
  margin-bottom: 24px;
}

.back-btn {
  background: none;
  border: none;
  font-size: 14px;
  color: #8b2c1d;
  cursor: pointer;
  padding: 0;
}

.back-btn:hover {
  color: #6e2216;
}

h1 {
  font-size: 24px;
  font-weight: normal;
  color: #4a3b2c;
  margin-bottom: 28px;
}

.filters {
  display: flex;
  gap: 0;
  margin-bottom: 32px;
  border-bottom: 1px solid #d4c5b0;
}

.filters button {
  padding: 10px 24px;
  background: none;
  border: none;
  font-size: 14px;
  cursor: pointer;
  color: #8b7a6b;
  border-bottom: 2px solid transparent;
}

.filters button.active {
  color: #8b2c1d;
  border-bottom-color: #8b2c1d;
}

.section {
  margin-bottom: 40px;
}

.section h2 {
  font-size: 18px;
  font-weight: normal;
  color: #4a3b2c;
  margin-bottom: 20px;
  padding-bottom: 8px;
  border-bottom: 1px solid #d4c5b0;
}

.card {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 20px;
  margin-bottom: 16px;
}

.card-content {
  flex: 1;
}

.request-text {
  font-size: 14px;
  color: #5c4d3e;
  margin-bottom: 16px;
}

.request-text strong {
  color: #4a3b2c;
}

.date {
  font-size: 12px;
  color: #8b7a6b;
  margin-bottom: 12px;
}

.actions {
  display: flex;
  gap: 12px;
  margin-top: 8px;
  flex-wrap: wrap;
}

.btn-confirm {
  padding: 8px 20px;
  background: #8b2c1d;
  border: none;
  color: white;
  font-size: 13px;
  cursor: pointer;
}

.btn-confirm:hover {
  background: #6e2216;
}

.btn-reject {
  padding: 8px 20px;
  background: white;
  border: 1px solid #d4c5b0;
  color: #5c4d3e;
  font-size: 13px;
  cursor: pointer;
}

.btn-reject:hover {
  background: #efe6db;
  border-color: #8b2c1d;
}

.btn-details {
  padding: 8px 20px;
  background: white;
  border: 1px solid #d4c5b0;
  color: #5c4d3e;
  font-size: 13px;
  cursor: pointer;
}

.btn-details:hover {
  background: #efe6db;
  border-color: #8b2c1d;
}

.status {
  display: inline-block;
  padding: 6px 14px;
  font-size: 12px;
}

.status-confirmed {
  background: white;
  border: 1px solid #d4c5b0;
  color: #5c4d3e;
}

.status-rejected {
  background: white;
  border: 1px solid #d4c5b0;
  color: #8b2c1d;
}

.status-pending {
  background: white;
  border: 1px solid #d4c5b0;
  color: #8b7a6b;
}

.status-with-action {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
  margin-top: 8px;
}

.btn-revoke {
  padding: 4px 12px;
  background: none;
  border: 1px solid #d4c5b0;
  font-size: 11px;
  cursor: pointer;
  color: #8b2c1d;
}

.btn-revoke:hover {
  background: #efe6db;
  border-color: #8b2c1d;
}

.empty {
  text-align: center;
  padding: 48px 24px;
  background: white;
  border: 1px solid #d4c5b0;
  color: #8b7a6b;
  font-size: 13px;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 28px;
  max-width: 400px;
  width: 90%;
}

.modal-content h3 {
  font-size: 18px;
  font-weight: normal;
  color: #4a3b2c;
  margin-bottom: 20px;
  padding-bottom: 12px;
  border-bottom: 1px solid #d4c5b0;
}

.modal-row {
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
  border-bottom: 1px solid #efe6db;
}

.modal-label {
  font-size: 13px;
  color: #8b7a6b;
}

.modal-value {
  font-size: 13px;
  color: #4a3b2c;
}

.modal-close-btn {
  width: 100%;
  padding: 10px;
  margin-top: 20px;
  background: #8b2c1d;
  border: none;
  color: white;
  font-size: 13px;
  cursor: pointer;
}

.modal-close-btn:hover {
  background: #6e2216;
}
</style>