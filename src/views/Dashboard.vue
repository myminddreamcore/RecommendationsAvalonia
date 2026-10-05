
<script setup>
import axios from 'axios';
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const user = ref(null);
const rating = ref(null);
const userSkills = ref([]);
const indexcomp = ref(0)
const confirmationsCount = ref(0);
const recommendationsCount = ref(0);
const profileCompleteness = ref(0);
const mobileMenuOpen = ref(false);
const doverie = ref('')
const ind = ref('')
const getToken = () => {
  const encrypted = localStorage.getItem('auth_token');
  if (!encrypted) return null;
  return atob(encrypted);
};

const loadProfile = async () => {
  const token = getToken()
  const userId = localStorage.getItem('user_id');
  
  if (!token || !userId) {
    router.push('/');
    return;
  }
  
  try {
    const response = await axios.get(`http://localhost:5015/User/users/me/${userId}`, {
      headers: { Authorization: `Bearer ${token}` }
    });
    user.value = response.data;
    
    const skillsResp = await axios.get(`http://localhost:5015/Skill/skillsuser/${userId}`);
    userSkills.value = skillsResp.data || [];
    const ratingResp = await axios.get(`http://localhost:5015/users/${userId}/rating`, {
      headers: { Authorization: `Bearer ${token}` }
    });
    rating.value = ratingResp.data;
    let j =0;
    for(let i =0; i<rating.value.levelRating/10; i++){
      doverie.value+="█"
      j+=1
    }
    for(j; j<10; j++){
      doverie.value+="░"
    }
    const index = await axios.post(`http://localhost:5015/users/${userId}/ratingskills`,null);
    indexcomp.value = index.data;
    let p =0;
    for(let i =0; i<indexcomp.value/10; i++){
      ind.value+="█"
      p+=1
    }
    for(p; p<10; p++){
      ind.value+="░"
    }
    console.log(doverie)
    const confirmResp = await axios.get(`http://localhost:5015/Confirmation/user/${userId}/confirmations`);
    confirmationsCount.value = confirmResp.data?.length || 0;
    
    let filled = 0;
    let total = 5;
    if (user.value.nameUser) filled++;
    if (user.value.surnameUser) filled++;
    if (user.value.phoneUser) filled++;
    if (userSkills.value.length > 0) filled++;
    if (user.value.emailUser) filled++;
    profileCompleteness.value = Math.round((filled / total) * 100);
    
    recommendationsCount.value = 3;
    
  } catch (err) {
    console.error(err);
    if (err.response?.status === 401) {
      localStorage.removeItem('auth_token');
      router.push('/');
    }
  }
};

const logout = async () => {
  const token = getToken();
  try {
    await axios.post('http://localhost:5260/User/auth/logout', {}, {
      headers: { Authorization: `Bearer ${token}` }
    });
  } catch (err) {}
  localStorage.removeItem('auth_token');
  localStorage.removeItem('user_id');
  router.push('/');
};

const editProfile = () => {
  router.push('/profile/edit');
};

const goToConfirmations = () => {
  router.push('/confirmations');
};

const exportPDF = () => {
  router.push('/export');
};

onMounted(() => {
  loadProfile();
});

const goTo = (path) => {
  router.push(path);
  mobileMenuOpen.value = false;
};
</script>
<template>
  <div class="app">
    <div class="sidebar">
      <div class="logo">Цифровой профиль</div>
      <nav>
        <p @click="goTo('/dashboard')" class="nav-item active">Главная</p>
        <p @click="goTo('/profile/edit')" class="nav-item">Редактировать профиль</p>
        <p @click="goTo('/confirmations')" class="nav-item">Подтверждения</p>
        <p @click="goTo('/rating')" class="nav-item">Рейтинг</p>
        <p @click="goTo('/export')" class="nav-item">Экспорт</p>
        <p @click="logout" class="nav-item logout">Выход</p>
      </nav>
    </div>

    <div class="main">
      <div class="mobile-header">
        <button @click="mobileMenuOpen = !mobileMenuOpen" class="menu-btn">☰</button>
        <h3>Главная</h3>
      </div>

      <div class="profile-card" v-if="user">
        <div class="avatar">
          <div class="avatar-placeholder">
            {{ user.nameUser?.charAt(0) }}{{ user.surnameUser?.charAt(0) }}
          </div>
        </div>
        <div class="profile-info">
          <h2>{{ user.surnameUser }} {{ user.nameUser }} {{ user.patronymicUser }}</h2>
          <p>Текущий рейтинг: {{ rating?.levelRating || 0 }} / 100</p>
          <div class="progress-bar">
            <div class="progress-fill" :style="{ width: profileCompleteness + '%' }"></div>
          </div>
          <p>Заполненность профиля: {{ profileCompleteness }}%</p>
        </div>
      </div>

      <div class="extra-metrics">
        <div class="extra-metric">Доверие {{ doverie }}</div>
        <div class="extra-metric">Индекс {{ ind }}</div>
      </div>

      <div class="metrics">
        <div class="metric-card">
          <h3>{{ confirmationsCount }}</h3>
          <p>Подтверждений</p>
        </div>
        <div class="metric-card">
          <h3>{{ userSkills.length }}</h3>
          <p>Навыков</p>
        </div>
        <div class="metric-card">
          <h3>{{ recommendationsCount }}</h3>
          <p>Рекомендаций</p>
        </div>
      </div>

      <div class="quick-actions">
        <button @click="editProfile" class="action-btn">Редактировать профиль</button>
        <button @click="goToConfirmations" class="action-btn">Подтверждения</button>
        <button @click="exportPDF" class="action-btn">Экспорт PDF</button>
      </div>

      <div class="breadcrumbs">
        <span>Главная</span>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      mobileMenuOpen: false,
      user: {
        nameUser: 'Иван',
        surnameUser: 'Иванов',
        patronymicUser: 'Иванович'
      },
      rating: {
        levelRating: 78
      },
      profileCompleteness: 65,
      doverie: 82,
      ind: 76,
      confirmationsCount: 24,
      userSkills: [{}, {}, {}],
      recommendationsCount: 12
    }
  },
  methods: {
    goTo(path) {},
    logout() {},
    editProfile() {},
    goToConfirmations() {},
    exportPDF() {}
  }
}
</script>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.app {
  display: flex;
  min-height: 100vh;
  background: #faf8f5;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
}

.sidebar {
  width: 260px;
  background: #faf8f5;
  border-right: 1px solid #d4c5b0;
  padding: 32px 24px;
}

.sidebar .logo {
  font-size: 18px;
  font-weight: normal;
  color: #8b2c1d;
  margin-bottom: 40px;
  letter-spacing: 0.5px;
}

.nav-item {
  padding: 10px 0;
  margin: 8px 0;
  cursor: pointer;
  color: #5c4d3e;
  font-size: 14px;
  border-bottom: 1px solid transparent;
  transition: none;
}

.nav-item.active {
  color: #8b2c1d;
  border-bottom-color: #8b2c1d;
}

.nav-item:hover {
  color: #8b2c1d;
}

.logout {
  margin-top: 60px;
}

.main {
  flex: 1;
  padding: 32px 40px;
  max-width: calc(100% - 260px);
}

.mobile-header {
  display: none;
  align-items: center;
  gap: 12px;
  margin-bottom: 24px;
}

.menu-btn {
  background: none;
  border: 1px solid #d4c5b0;
  font-size: 20px;
  padding: 6px 12px;
  cursor: pointer;
  color: #5c4d3e;
}

.profile-card {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 28px;
  display: flex;
  gap: 28px;
  margin-bottom: 24px;
  flex-wrap: wrap;
}

.avatar-placeholder {
  width: 80px;
  height: 80px;
  background: #8b2c1d;
  color: white;
  font-size: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0;
}

.profile-info h2 {
  font-size: 20px;
  font-weight: normal;
  color: #4a3b2c;
  margin-bottom: 12px;
}

.profile-info p {
  font-size: 13px;
  color: #8b7a6b;
  margin-bottom: 8px;
}

.progress-bar {
  width: 260px;
  height: 4px;
  background: #e8dfd4;
  overflow: hidden;
  margin: 8px 0;
}

.progress-fill {
  height: 100%;
  background: #8b2c1d;
  transition: width 0.3s;
}

.extra-metrics {
  display: flex;
  gap: 20px;
  margin-bottom: 28px;
  flex-wrap: wrap;
}

.extra-metric {
  font-size: 13px;
  color: #5c4d3e;
  background: white;
  border: 1px solid #d4c5b0;
  padding: 8px 16px;
}

.metrics {
  display: flex;
  gap: 20px;
  margin-bottom: 28px;
  flex-wrap: wrap;
}

.metric-card {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 20px 28px;
  text-align: center;
  flex: 1;
  min-width: 110px;
}

.metric-card h3 {
  font-size: 32px;
  font-weight: normal;
  color: #8b2c1d;
  margin: 0 0 8px 0;
}

.metric-card p {
  font-size: 12px;
  color: #8b7a6b;
  letter-spacing: 0.5px;
}

.quick-actions {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
  margin-bottom: 32px;
}

.action-btn {
  padding: 10px 24px;
  background: white;
  border: 1px solid #d4c5b0;
  font-size: 13px;
  cursor: pointer;
  color: #5c4d3e;
}

.action-btn:hover {
  background: #efe6db;
  border-color: #8b2c1d;
}

.breadcrumbs {
  margin-top: 20px;
  font-size: 12px;
  color: #8b7a6b;
}

@media (max-width: 768px) {
  .sidebar {
    display: none;
  }

  .mobile-header {
    display: flex;
  }

  .main {
    margin-left: 0;
    max-width: 100%;
    padding: 20px;
  }

  .profile-card {
    flex-direction: column;
    text-align: center;
    align-items: center;
  }

  .progress-bar {
    margin: 8px auto;
  }
}
</style>