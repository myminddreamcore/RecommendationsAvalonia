<script setup>
import axios from 'axios';
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const token = ref('');
const userId = ref('');
const user = ref(null);
const userSkills = ref([]);
const userExperience = ref([]);

const exportFormat = ref('pdf');
const expiryDays = ref('7');
const privacyMode = ref('full');
const generating = ref(false);
const generatedLink = ref('');
const countdown = ref('');
let countdownInterval = null;

const getToken = () => {
  const encrypted = localStorage.getItem('auth_token');
  if (!encrypted) return null;
  return atob(encrypted);
};

const loadProfileForPreview = async () => {
  token.value = getToken();
  userId.value = localStorage.getItem('user_id');
  
  try {
    const userResp = await axios.get(`http://localhost:5015/User/users/me/${userId.value}`, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    user.value = userResp.data;
    
    const skillsResp = await axios.post(`http://localhost:5015/User/users/${userId.value}/skills`, {});
    userSkills.value = (skillsResp.data || []).slice(0, 3);
    
    const expResp = await axios.post(`http://localhost:5015/User/users/${userId.value}/experience`, {});
    userExperience.value = (expResp.data || []).slice(0, 2);
  } catch (err) {
    console.error(err);
  }
};

const previewContent = computed(() => {
  if (privacyMode.value === 'anon') {
    return {
      name: 'Скрыто',
      rating: '***',
      skills: ['Скрыто', 'Скрыто', 'Скрыто'],
      experience: ['Опыт скрыт']
    };
  }
  
  return {
    name: `${user.value?.surnameUser} ${user.value?.nameUser}`,
    rating: '75 / 100',
    skills: userSkills.value.map(s => s.idSkillNavigation?.nameSkill || 'Навык'),
    experience: userExperience.value.map(e => `${e.post} в ${e.nameCompany}`)
  };
});

const generateExport = async () => {
  generating.value = true;
  generatedLink.value = '';
  
  try {
    const response = await axios.get('http://localhost:5015/users/me/export', {
      params: { format: exportFormat.value },
      responseType: 'blob'
    });
    
    const mimeType = exportFormat.value === 'pdf' ? 'application/pdf' : 'application/json';
    const blob = new Blob([response.data], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `profile_${new Date().toISOString().slice(0, 10)}.${exportFormat.value}`;
    a.click();
    URL.revokeObjectURL(url);
    
    generatedLink.value = `http://localhost:5015/users/me/export`;
    
    if (expiryDays.value !== '0') {
      let seconds = parseInt(expiryDays.value) * 24 * 60 * 60;
      if (countdownInterval) clearInterval(countdownInterval);
      countdownInterval = setInterval(() => {
        if (seconds <= 0) {
          clearInterval(countdownInterval);
          countdown.value = 'Срок действия истек';
        } else {
          const days = Math.floor(seconds / 86400);
          const hours = Math.floor((seconds % 86400) / 3600);
          countdown.value = `${days} дн ${hours} ч`;
          seconds--;
        }
      }, 1000);
    } else {
      countdown.value = 'Бессрочно';
    }
  } catch (err) {
    alert('Ошибка генерации. Попробуйте еще раз.');
  } finally {
    generating.value = false;
  }
};

const copyLink = () => {
  navigator.clipboard.writeText(generatedLink.value);
  alert('Ссылка скопирована');
};

onMounted(() => {
  loadProfileForPreview();
});
</script>

<template>
  <div class="export-container">
    <div class="back">
      <button @click="router.push('/dashboard')" class="back-btn">← Назад</button>
    </div>

    <div class="breadcrumbs">
      <span class="breadcrumb-item">Профиль</span>
      <span class="breadcrumb-sep">→</span>
      <span class="breadcrumb-item active">Экспорт</span>
    </div>

    <h1>Экспорт профиля</h1>

    <div class="export-form">
      <div class="field">
        <label class="field-label">Формат экспорта</label>
        <div class="radio-group">
          <label class="radio-label">
            <input type="radio" value="pdf" v-model="exportFormat" class="radio-input" />
            <span class="radio-text">PDF (полный профиль)</span>
          </label>
          <label class="radio-label">
            <input type="radio" value="json" v-model="exportFormat" class="radio-input" />
            <span class="radio-text">JSON (структурированные данные)</span>
          </label>
        </div>
      </div>

      <div class="field">
        <label class="field-label">Срок действия ссылки</label>
        <select v-model="expiryDays" class="form-select">
          <option value="7">7 дней</option>
          <option value="30">30 дней</option>
          <option value="0">Бессрочно</option>
        </select>
      </div>

      <div class="field">
        <label class="field-label">Режим приватности</label>
        <div class="radio-group">
          <label class="radio-label">
            <input type="radio" value="full" v-model="privacyMode" class="radio-input" />
            <span class="radio-text">Полный профиль</span>
          </label>
          <label class="radio-label">
            <input type="radio" value="anon" v-model="privacyMode" class="radio-input" />
            <span class="radio-text">Анонимный (для работодателей)</span>
          </label>
        </div>
      </div>

      <div class="preview">
        <p class="preview-title">Предпросмотр</p>
        <div class="preview-content">
          <div class="preview-row">
            <span class="preview-label">Название профиля</span>
            <span class="preview-value">{{ previewContent.name }}</span>
          </div>
          <div class="preview-row">
            <span class="preview-label">Рейтинг</span>
            <span class="preview-value">{{ previewContent.rating }}</span>
          </div>
          <div class="preview-row">
            <span class="preview-label">Топ-3 навыков</span>
            <span class="preview-value">{{ previewContent.skills.join(', ') }}</span>
          </div>
          <div class="preview-row">
            <span class="preview-label">Опыт работы</span>
            <span class="preview-value">{{ previewContent.experience.join('; ') }}</span>
          </div>
        </div>
      </div>

      <button @click="generateExport" :disabled="generating" class="btn-generate">
        {{ generating ? 'Генерация...' : 'Генерировать' }}
      </button>

      <div v-if="generatedLink" class="result">
        <div class="result-icon"></div>
        <p class="result-title">Файл успешно создан</p>
        <div class="result-link">
          <span class="link-label">Ссылка</span>
          <a :href="generatedLink" target="_blank" class="link-value">{{ generatedLink }}</a>
        </div>
        <button @click="copyLink" class="btn-copy">Копировать ссылку</button>
        <p v-if="countdown" class="result-expiry">Срок действия: {{ countdown }}</p>
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

.export-container {
  max-width: 680px;
  margin: 0 auto;
  padding: 32px 28px;
  background: #faf8f5;
  min-height: 100vh;
}

.back {
  margin-bottom: 16px;
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

.breadcrumbs {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 24px;
  font-size: 13px;
}

.breadcrumb-item {
  color: #8b7a6b;
}

.breadcrumb-item.active {
  color: #4a3b2c;
}

.breadcrumb-sep {
  color: #d4c5b0;
}

h1 {
  font-size: 24px;
  font-weight: normal;
  color: #4a3b2c;
  margin-bottom: 28px;
}

.export-form {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 28px;
}

.field {
  margin-bottom: 28px;
}

.field-label {
  display: block;
  font-size: 14px;
  color: #5c4d3e;
  margin-bottom: 12px;
}

.radio-group {
  display: flex;
  gap: 24px;
  flex-wrap: wrap;
}

.radio-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.radio-input {
  width: 16px;
  height: 16px;
  cursor: pointer;
  accent-color: #8b2c1d;
}

.radio-text {
  font-size: 14px;
  color: #5c4d3e;
}

.form-select {
  width: 100%;
  padding: 10px 12px;
  background: white;
  border: 1px solid #d4c5b0;
  font-size: 14px;
  color: #5c4d3e;
  cursor: pointer;
}

.form-select:focus {
  outline: none;
  border-color: #8b2c1d;
}

.preview {
  background: #faf8f5;
  border: 1px solid #e8dfd4;
  padding: 20px;
  margin: 24px 0;
}

.preview-title {
  font-size: 14px;
  color: #4a3b2c;
  margin-bottom: 16px;
  padding-bottom: 10px;
  border-bottom: 1px solid #e8dfd4;
}

.preview-content {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.preview-row {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  font-size: 13px;
}

.preview-label {
  width: 130px;
  color: #8b7a6b;
  flex-shrink: 0;
}

.preview-value {
  color: #5c4d3e;
  word-break: break-word;
}

.btn-generate {
  width: 100%;
  padding: 12px;
  background: #8b2c1d;
  border: none;
  color: white;
  font-size: 15px;
  cursor: pointer;
  margin-top: 8px;
}

.btn-generate:hover:not(:disabled) {
  background: #6e2216;
}

.btn-generate:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.result {
  margin-top: 24px;
  padding: 20px;
  background: white;
  border: 1px solid #d4c5b0;
}

.result-icon {
  width: 40px;
  height: 40px;
  background: #e8dfd4;
  margin-bottom: 12px;
  position: relative;
}

.result-icon::before {
  content: "✓";
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 20px;
  color: #8b2c1d;
}

.result-title {
  font-size: 16px;
  color: #4a3b2c;
  margin-bottom: 12px;
}

.result-link {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-bottom: 16px;
}

.link-label {
  font-size: 12px;
  color: #8b7a6b;
}

.link-value {
  font-size: 13px;
  color: #8b2c1d;
  text-decoration: none;
  word-break: break-all;
}

.link-value:hover {
  text-decoration: underline;
}

.btn-copy {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 8px 20px;
  font-size: 13px;
  cursor: pointer;
  color: #5c4d3e;
}

.btn-copy:hover {
  background: #faf8f5;
  border-color: #8b2c1d;
}

.result-expiry {
  margin-top: 12px;
  font-size: 12px;
  color: #8b7a6b;
}

@media (max-width: 560px) {
  .export-container {
    padding: 20px 16px;
  }
  
  .export-form {
    padding: 20px;
  }
  
  .radio-group {
    gap: 16px;
  }
  
  .preview-row {
    flex-direction: column;
    gap: 4px;
  }
  
  .preview-label {
    width: auto;
  }
}
</style>