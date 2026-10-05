<script setup>
import axios from 'axios';
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const token = ref('');
const userId = ref('');

const rating = ref(null);
const ratingHistory = ref([]);
const period = ref('30');
const recommendations = ref([]);
const vacancies = ref([]);
const loading = ref(false);

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

const loadData = async () => {
  loading.value = true;
  token.value = getToken();
  userId.value = localStorage.getItem('user_id');
  
  try {
    const ratingResp = await axios.get(`http://localhost:5015/users/${userId.value}/rating`, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    rating.value = ratingResp.data;
    
    const historyResp = await axios.post(`http://localhost:5015/users/me/rating/history/${userId.value}/${period.value}`, {});
    ratingHistory.value = historyResp.data || [];
    
    const recResp = await axios.post(`http://localhost:5015/User/users/${userId.value}/recommendations`, {});
    recommendations.value = recResp.data || [];
    vacancies.value = recResp.data?.vacancies || [];
  } catch (err) {
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const changePeriod = () => {
  loadData();
};

const getGraphBars = () => {
  if (!ratingHistory.value.length) return [];
  const max = Math.max(...ratingHistory.value.map(r => r.levelRating || 0), 100);
  return ratingHistory.value.map(r => ({
    ...r,
    height: ((r.levelRating || 0) / max) * 50
  }));
};

onMounted(() => {
  loadData();
});
</script>

<template>
  <div class="rating-container">
    <div class="back">
      <button @click="router.push('/dashboard')" class="back-btn">← Назад</button>
    </div>

    <h1>Рейтинг и рекомендации</h1>

    <div v-if="loading" class="loading-state">
      <div class="loading-spinner"></div>
      <p>Загрузка...</p>
    </div>

    <div v-else>
      <div class="rating-block">
        <h2>Индекс компетенций</h2>
        <div class="rating-value">
          <span class="big-number">{{ rating?.levelRating || 0 }}</span>
          <span class="rating-max">/ 100</span>
        </div>

        <div class="pie-chart">
          <div class="pie-container">
            <div class="pie">
              <div class="pie-segment edu"></div>
              <div class="pie-segment exp"></div>
              <div class="pie-segment conf"></div>
              <div class="pie-inner"></div>
            </div>
          </div>
          <div class="pie-legend">
            <div class="legend-item">
              <span class="legend-color color-edu"></span>
              <span class="legend-text">Образование 30%</span>
            </div>
            <div class="legend-item">
              <span class="legend-color color-exp"></span>
              <span class="legend-text">Опыт 30%</span>
            </div>
            <div class="legend-item">
              <span class="legend-color color-conf"></span>
              <span class="legend-text">Подтверждения 40%</span>
            </div>
          </div>
        </div>
      </div>

      <div class="graph-block">
        <div class="graph-header">
          <h2>Динамика рейтинга</h2>
          <select v-model="period" @change="changePeriod" class="period-select">
            <option value="30">30 дней</option>
            <option value="90">90 дней</option>
          </select>
        </div>

        <div class="graph" v-if="ratingHistory.length">
          <div class="graph-bars">
            <div v-for="(item, idx) in getGraphBars()" :key="idx" class="graph-bar-container">
              <div class="graph-bar" :style="{ height: item.height + 'px' }"></div>
              <span class="graph-label">{{ formatDate(item.dateRating).slice(0, 5) }}</span>
            </div>
          </div>
        </div>
        <div v-else class="empty-state">Рейтинг формируется</div>
      </div>

      <div class="recommendations-block">
        <h2>Рекомендации по развитию</h2>

        <div v-for="rec in recommendations" :key="rec.skillName" class="rec-card">
          <div class="rec-header">
            <strong class="rec-title">Подтянуть {{ rec.skillName }}</strong>
            <span class="rec-required">требуется {{ rec.required }}/10, сейчас {{ rec.current }}/10</span>
          </div>
          <div class="skill-progress">
            <div class="progress-fill" :style="{ width: (rec.current / rec.required) * 100 + '%' }"></div>
          </div>
        </div>

        <div v-for="vac in vacancies" :key="vac.title" class="vac-card">
          <div class="vac-header">
            <strong class="vac-title">{{ vac.title }}</strong>
            <span class="vac-match">{{ vac.match }}% соответствие</span>
          </div>
          <div class="skill-progress">
            <div class="progress-fill" :style="{ width: vac.match + '%' }"></div>
          </div>
        </div>

        <div v-if="!recommendations.length && !vacancies.length" class="empty-state">
          Рекомендации появятся после заполнения профиля
        </div>
      </div>

      <div class="history-block">
        <h2>История изменений рейтинга</h2>

        <div class="table-wrapper" v-if="ratingHistory.length">
          <table class="rating-table">
            <thead>
              <tr>
                <th>Дата</th>
                <th>Значение рейтинга</th>
                <th>Тип изменения</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in ratingHistory" :key="item.idRating">
                <td>{{ formatDate(item.dateRating) }}</td>
                <td class="rating-value-cell">{{ item.levelRating }}</td>
                <td>{{ item.typeRating || 'Обновление' }}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div v-else class="empty-state">Рейтинг формируется</div>
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

.rating-container {
  max-width: 1000px;
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

h2 {
  font-size: 18px;
  font-weight: normal;
  color: #4a3b2c;
  margin-bottom: 20px;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px;
  background: white;
  border: 1px solid #d4c5b0;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 2px solid #d4c5b0;
  border-top-color: #8b2c1d;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.rating-block {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 28px;
  margin-bottom: 28px;
  text-align: center;
}

.rating-value {
  margin: 20px 0 28px;
}

.big-number {
  font-size: 64px;
  font-weight: normal;
  color: #8b2c1d;
}

.rating-max {
  font-size: 20px;
  color: #8b7a6b;
}

.pie-chart {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 48px;
  flex-wrap: wrap;
}

.pie-container {
  position: relative;
}

.pie {
  width: 160px;
  height: 160px;
  border-radius: 50%;
  position: relative;
  overflow: hidden;
  background: #e8dfd4;
}

.pie-segment {
  position: absolute;
  width: 100%;
  height: 100%;
  left: 0;
  top: 0;
}

.pie-segment.edu {
  background: #10b981;
  clip: rect(0, 160px, 160px, 80px);
  transform: rotate(0deg);
}

.pie-segment.exp {
  background: #f59e0b;
  clip: rect(0, 160px, 160px, 80px);
  transform: rotate(108deg);
}

.pie-segment.conf {
  background: #3b82f6;
  clip: rect(0, 160px, 160px, 80px);
  transform: rotate(216deg);
}

.pie-inner {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 80px;
  height: 80px;
  background: white;
  border-radius: 50%;
}

.pie-legend {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 10px;
}

.legend-color {
  width: 14px;
  height: 14px;
}

.legend-color.color-edu {
  background: #10b981;
}

.legend-color.color-exp {
  background: #f59e0b;
}

.legend-color.color-conf {
  background: #3b82f6;
}

.legend-text {
  font-size: 14px;
  color: #5c4d3e;
}

.graph-block {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 28px;
  margin-bottom: 28px;
}

.graph-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
  margin-bottom: 24px;
}

.period-select {
  padding: 8px 16px;
  background: white;
  border: 1px solid #d4c5b0;
  font-size: 13px;
  color: #5c4d3e;
  cursor: pointer;
}

.graph-bars {
  display: flex;
  gap: 6px;
  align-items: flex-end;
  height: 200px;
  margin-top: 20px;
  overflow-x: auto;
  padding-bottom: 8px;
}

.graph-bar-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 36px;
}

.graph-bar {
  width: 24px;
  background: #8b2c1d;
  border-radius: 2px 2px 0 0;
  transition: height 0.3s;
}

.graph-label {
  font-size: 10px;
  color: #8b7a6b;
  margin-top: 8px;
}

.recommendations-block {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 28px;
  margin-bottom: 28px;
}

.rec-card, .vac-card {
  padding: 16px 20px;
  margin-bottom: 12px;
  background: #faf8f5;
  border: 1px solid #e8dfd4;
}

.rec-header, .vac-header {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 12px;
}

.rec-title, .vac-title {
  font-size: 14px;
  color: #4a3b2c;
}

.rec-required {
  font-size: 12px;
  color: #8b7a6b;
}

.vac-match {
  font-size: 13px;
  color: #8b2c1d;
}

.skill-progress {
  height: 6px;
  background: #e8dfd4;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: #8b2c1d;
  transition: width 0.3s;
}

.history-block {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 28px;
}

.table-wrapper {
  overflow-x: auto;
}

.rating-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
}

.rating-table th {
  text-align: left;
  padding: 12px 12px;
  background: #faf8f5;
  color: #5c4d3e;
  font-weight: normal;
  border-bottom: 1px solid #d4c5b0;
}

.rating-table td {
  padding: 12px 12px;
  color: #5c4d3e;
  border-bottom: 1px solid #e8dfd4;
}

.rating-table tr:last-child td {
  border-bottom: none;
}

.rating-value-cell {
  color: #8b2c1d;
}

.empty-state {
  text-align: center;
  padding: 48px 24px;
  background: white;
  border: 1px solid #d4c5b0;
  color: #8b7a6b;
  font-size: 13px;
}
</style>