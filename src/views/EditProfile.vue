
<script setup>
import axios from 'axios';
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import Draggable from 'vuedraggable';
const router = useRouter();
const activeTab = ref('common');

const token = ref('');
const userId = ref('');

const user = ref({
  nameUser: '',
  surnameUser: '',
  patronymicUser: '',
  phoneUser: '',
  emailUser: ''
});

const educations = ref([]);
const newEducation = ref({ type: 'ВУЗ', name: '', dateStart: '', dateEnd: '', result: '' });

const experiences = ref([]);
const newExperience = ref({ type: 'ТК РФ', company: '', post: '', dateStart: '', dateEnd: '' });
const expError = ref('');

const allSkills = ref([]);
const userSkills = ref([]);
const skillSearch = ref('');
const skillSuggestions = ref([]);
const newSkill = ref({ name: '', level: 5 });

const getToken = () => {
  const encrypted = localStorage.getItem('auth_token');
  if (!encrypted) return null;
  return atob(encrypted);
};

const loadData = async () => {
  token.value = getToken();
  userId.value = localStorage.getItem('user_id');
  
  if (!token.value || !userId.value) {
    router.push('/');
    return;
  }
  
  try {
    const userResp = await axios.get(`http://localhost:5015/User/users/me/${userId.value}`, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    user.value = userResp.data;
    
    const eduResp = await axios.post(`http://localhost:5015/User/users/${userId.value}/education`,null);
    educations.value = eduResp.data || [];
    
    const expResp = await axios.post(`http://localhost:5015/User/users/${userId.value}/experience`, null);
    experiences.value = expResp.data || [];
    
    const skillsResp = await axios.post(`http://localhost:5015/User/users/${userId.value}/skills`, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    userSkills.value = skillsResp.data || [];
    
    const allSkillsResp = await axios.get('http://localhost:5015/Skill/skillsall');
    allSkills.value = allSkillsResp.data || [];
    
  } catch (err) {
    console.error(err);
  }
};

const saveCommon = async () => {
  try {
    await axios.put('http://localhost:5015/User/users/me', user.value, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    alert('Данные сохранены');
  } catch (err) {
    alert('Ошибка сохранения');
  }
};

const addEducation = async () => {
  const edu = {
    nameEducation: newEducation.value.name,
    typeEductaion: newEducation.value.type,
    dateStart: newEducation.value.dateStart,
    dateEnd: newEducation.value.dateEnd,
    result: newEducation.value.result,
    idUser: parseInt(userId.value)
  };
  
  try {
    const resp = await axios.post('http://localhost:5015/User/users/me/education', edu, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    educations.value.push(resp.data);
    newEducation.value = { type: 'ВУЗ', name: '', dateStart: '', dateEnd: '', result: '' };
  } catch (err) {
    alert('Ошибка добавления');
  }
};

const deleteEducation = async (id) => {
  if (confirm('Удалить?')) {
    await axios.delete(`http://localhost:5015/User/users/me/education/${id}`, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    educations.value = educations.value.filter(e => e.idEducation !== id);
  }
};


const addExperience = async () => {
  expError.value = '';
  
  const start = new Date(newExperience.value.dateStart);
  const end = new Date(newExperience.value.dateEnd);
  
  for (let exp of experiences.value) {
    const expStart = new Date(exp.dateStart);
    const expEnd = new Date(exp.dateEnd);
    
    if ((start >= expStart && start <= expEnd) || 
        (end >= expStart && end <= expEnd) ||
        (start <= expStart && end >= expEnd)) {
      expError.value = 'Периоды опыта не должны пересекаться!';
      return;
    }
  }
  
  const expData = {
    type: newExperience.value.type,
    nameCompany: newExperience.value.company,
    post: newExperience.value.post,
    dateStart: newExperience.value.dateStart,
    dateEnd: newExperience.value.dateEnd,
    idUser: parseInt(userId.value)
  };
  
  try {
    const resp = await axios.post('http://localhost:5015/User/users/me/experience', expData, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    experiences.value.push(resp.data);
    newExperience.value = { type: 'ТК РФ', company: '', post: '', dateStart: '', dateEnd: '' };
  } catch (err) {
    alert('Ошибка добавления');
  }
};

const deleteExperience = async (id) => {
  if (confirm('Удалить?')) {
    await axios.delete(`http://localhost:5015/User/users/me/experience/${id}`, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    experiences.value = experiences.value.filter(e => e.id !== id);
  }
};

const searchSkills = () => {
  if (skillSearch.value.length < 2) {
    skillSuggestions.value = [];
    return;
  }
  
  skillSuggestions.value = allSkills.value.filter(s => 
    s.nameSkill.toLowerCase().includes(skillSearch.value.toLowerCase())
  ).slice(0, 5);
};

const selectSkill = (skill) => {
  skillSearch.value = skill.nameSkill;
  skillSuggestions.value = [];
};

const addSkill = async () => {
  if (!skillSearch.value) return;
  
  const exists = userSkills.value.some(s => 
    s.idSkillNavigation?.nameSkill?.toLowerCase() === skillSearch.value.toLowerCase()
  );
  
  if (exists) {
    alert('Навык уже добавлен');
    return;
  }
  
  let skillId = allSkills.value.find(s => 
    s.nameSkill.toLowerCase() === skillSearch.value.toLowerCase()
  )?.idSkill;
  
  if (!skillId) {
    alert('Навык не найден в базе');
    return;
  }
  
  const skillData = {
    idSkill: skillId,
    markSkill: newSkill.value.level,
    userId: parseInt(userId.value),
    status: 'active'
  };
  
  try {
    const resp = await axios.post('http://localhost:5015/User/users/me/skills', skillData, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    userSkills.value.push(resp.data);
    skillSearch.value = '';
    newSkill.value.level = 5;
  } catch (err) {
    alert('Ошибка добавления навыка');
  }
};

const updateSkillLevel = async (skill, level) => {
  skill.markSkill = level;
  try {
    await axios.put(`http://localhost:5015/User/users/me/skills/${skill.id}`, skill, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
  } catch (err) {}
};

const deleteSkill = async (id) => {
  if (confirm('Удалить навык?')) {
    await axios.delete(`http://localhost:5015/User/users/me/skills/${id}`, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    userSkills.value = userSkills.value.filter(s => s.id !== id);
  }
};

onMounted(() => {
  loadData();
});
</script>
<template>
  <div class="edit-container">
    <div class="back">
      <button @click="goTo('/dashboard')" class="back-btn">← Назад</button>
    </div>

    <h1>Редактирование профиля</h1>

    <div class="tabs">
      <button @click="activeTab='common'" :class="{active: activeTab==='common'}">Общие данные</button>
      <button @click="activeTab='education'" :class="{active: activeTab==='education'}">Образование</button>
      <button @click="activeTab='experience'" :class="{active: activeTab==='experience'}">Опыт работы</button>
      <button @click="activeTab='skills'" :class="{active: activeTab==='skills'}">Навыки</button>
    </div>

    <div v-if="activeTab==='common'">
      <div class="field">
        <p>Имя</p>
        <input v-model="user.nameUser" />
      </div>
      <div class="field">
        <p>Фамилия</p>
        <input v-model="user.surnameUser" />
      </div>
      <div class="field">
        <p>Отчество</p>
        <input v-model="user.patronymicUser" />
      </div>
      <div class="field">
        <p>Телефон</p>
        <input v-model="user.phoneUser" />
      </div>
      <div class="field">
        <p>Email</p>
        <input v-model="user.emailUser" readonly />
      </div>
      <button @click="saveCommon" class="save-btn">Сохранить</button>
    </div>

    <div v-if="activeTab==='education'">
      <div class="add-form">
        <h3>Добавить образование</h3>
        <select v-model="newEducation.type">
          <option>ВУЗ</option>
          <option>Курсы</option>
        </select>
        <input placeholder="Название" v-model="newEducation.name" />
        <input type="date" placeholder="Дата начала" v-model="newEducation.dateStart" />
        <input type="date" placeholder="Дата окончания" v-model="newEducation.dateEnd" />
        <input placeholder="Документ" v-model="newEducation.result" />
        <button @click="addEducation" class="add-btn">Добавить</button>
      </div>

      <div class="items-list">
        <div v-for="edu in educations" :key="edu.idEducation" class="item-card">
          <div class="item-content">
            <p class="item-title"><strong>{{ edu.typeEductaion }}</strong> - {{ edu.nameEducation }}</p>
            <p class="item-date">{{ edu.dateStart }} — {{ edu.dateEnd }}</p>
            <p class="item-doc">Документ: {{ edu.result }}</p>
          </div>
          <button @click="deleteEducation(edu.idEducation)" class="del-btn">Удалить</button>
        </div>
      </div>
    </div>

    <div v-if="activeTab==='experience'">
      <div class="add-form">
        <h3>Добавить опыт</h3>
        <select v-model="newExperience.type">
          <option>ТК РФ</option>
          <option>фриланс</option>
          <option>ИП</option>
        </select>
        <input placeholder="Компания" v-model="newExperience.company" />
        <input placeholder="Должность" v-model="newExperience.post" />
        <input type="date" placeholder="Дата начала" v-model="newExperience.dateStart" />
        <input type="date" placeholder="Дата окончания" v-model="newExperience.dateEnd" />
        <p v-if="expError" class="error">{{ expError }}</p>
        <button @click="addExperience" class="add-btn">Добавить</button>
      </div>

      <div class="items-list">
        <div v-for="exp in experiences" :key="exp.id" class="item-card">
          <div class="item-content">
            <p class="item-title"><strong>{{ exp.type }}</strong> - {{ exp.nameCompany }} ({{ exp.post }})</p>
            <p class="item-date">{{ exp.dateStart }} — {{ exp.dateEnd }}</p>
          </div>
          <button @click="deleteExperience(exp.id)" class="del-btn">Удалить</button>
        </div>
      </div>
    </div>

    <div v-if="activeTab==='skills'">
      <div class="add-form">
        <h3>Добавить навык</h3>
        <div class="autocomplete">
          <input placeholder="Поиск технологии" v-model="skillSearch" @input="searchSkills" class="skill-search" />
          <div class="suggestions" v-if="skillSuggestions.length">
            <div v-for="s in skillSuggestions" @click="selectSkill(s)" class="suggestion">
              {{ s.nameSkill }}
            </div>
          </div>
        </div>
        <div class="level-control">
          <span class="level-label">Уровень: {{ newSkill.level }} / 10</span>
          <input type="range" min="0" max="10" v-model="newSkill.level" class="slider" />
        </div>
        <button @click="addSkill" class="add-btn">Добавить навык</button>
      </div>

      <div class="skills-list">
        <div v-for="skill in userSkills" :key="skill.id" class="skill-card">
          <div class="skill-info">
            <strong class="skill-name">{{ skill.idSkillNavigation?.nameSkill }}</strong>
            <div class="skill-level-row">
              <span class="level-text">Уровень: {{ skill.markSkill }}/10</span>
              <input type="range" min="0" max="10" v-model="skill.markSkill" 
                     @change="updateSkillLevel(skill, skill.markSkill)" class="slider-small" />
            </div>
          </div>
          <button @click="deleteSkill(skill.id)" class="del-btn">Удалить</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      activeTab: 'common',
      user: {
        nameUser: '',
        surnameUser: '',
        patronymicUser: '',
        phoneUser: '',
        emailUser: 'user@example.com'
      },
      newEducation: {
        type: 'ВУЗ',
        name: '',
        dateStart: '',
        dateEnd: '',
        result: ''
      },
      educations: [],
      newExperience: {
        type: 'ТК РФ',
        company: '',
        post: '',
        dateStart: '',
        dateEnd: ''
      },
      experiences: [],
      expError: '',
      skillSearch: '',
      skillSuggestions: [],
      newSkill: {
        level: 5
      },
      userSkills: []
    }
  },
  methods: {
    goTo(path) {
      this.$router.push(path)
    },
    saveCommon() {},
    addEducation() {},
    deleteEducation(id) {},
    addExperience() {},
    deleteExperience(id) {},
    searchSkills() {},
    selectSkill(skill) {},
    addSkill() {},
    updateSkillLevel(skill, level) {},
    deleteSkill(id) {}
  }
}
</script>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.edit-container {
  max-width: 860px;
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

.tabs {
  display: flex;
  gap: 0;
  margin-bottom: 32px;
  border-bottom: 1px solid #d4c5b0;
  flex-wrap: wrap;
}

.tabs button {
  padding: 10px 24px;
  background: none;
  border: none;
  font-size: 14px;
  cursor: pointer;
  color: #8b7a6b;
  border-bottom: 2px solid transparent;
}

.tabs button.active {
  color: #8b2c1d;
  border-bottom-color: #8b2c1d;
}

.field {
  margin-bottom: 20px;
}

.field p {
  margin-bottom: 8px;
  font-size: 13px;
  color: #5c4d3e;
}

.field input {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #d4c5b0;
  background: white;
  font-size: 14px;
}

.field input:focus {
  outline: none;
  border-color: #8b2c1d;
}

.field input[readonly] {
  background: #efe6db;
}

.save-btn, .add-btn {
  width: 100%;
  padding: 12px;
  background: #8b2c1d;
  border: none;
  color: white;
  font-size: 14px;
  cursor: pointer;
  margin-top: 8px;
}

.save-btn:hover, .add-btn:hover {
  background: #6e2216;
}

.add-form {
  background: white;
  border: 1px solid #d4c5b0;
  padding: 20px;
  margin-bottom: 28px;
}

.add-form h3 {
  font-size: 16px;
  font-weight: normal;
  color: #4a3b2c;
  margin-bottom: 16px;
}

.add-form select, .add-form input {
  width: 100%;
  padding: 10px 12px;
  margin-bottom: 12px;
  border: 1px solid #d4c5b0;
  background: white;
  font-size: 14px;
}

.add-form select:focus, .add-form input:focus {
  outline: none;
  border-color: #8b2c1d;
}

.items-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.item-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: white;
  border: 1px solid #d4c5b0;
  padding: 16px 20px;
}

.item-content {
  flex: 1;
}

.item-title {
  font-size: 14px;
  color: #4a3b2c;
  margin-bottom: 6px;
}

.item-date {
  font-size: 12px;
  color: #8b7a6b;
  margin-bottom: 4px;
}

.item-doc {
  font-size: 12px;
  color: #8b7a6b;
}

.del-btn {
  background: none;
  border: 1px solid #d4c5b0;
  padding: 6px 16px;
  font-size: 12px;
  cursor: pointer;
  color: #8b2c1d;
}

.del-btn:hover {
  background: #efe6db;
  border-color: #8b2c1d;
}

.skill-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: white;
  border: 1px solid #d4c5b0;
  padding: 16px 20px;
  margin-bottom: 10px;
}

.skill-info {
  flex: 1;
}

.skill-name {
  font-size: 14px;
  color: #4a3b2c;
  display: block;
  margin-bottom: 10px;
}

.skill-level-row {
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}

.level-text {
  font-size: 12px;
  color: #8b7a6b;
}

.level-control {
  margin: 16px 0;
}

.level-label {
  font-size: 13px;
  color: #5c4d3e;
  display: block;
  margin-bottom: 10px;
}

.slider {
  width: 100%;
  height: 4px;
  -webkit-appearance: none;
  background: #e8dfd4;
}

.slider::-webkit-slider-thumb {
  -webkit-appearance: none;
  width: 16px;
  height: 16px;
  background: #8b2c1d;
  cursor: pointer;
}

.slider-small {
  width: 160px;
  height: 4px;
  -webkit-appearance: none;
  background: #e8dfd4;
}

.slider-small::-webkit-slider-thumb {
  -webkit-appearance: none;
  width: 14px;
  height: 14px;
  background: #8b2c1d;
  cursor: pointer;
}

.autocomplete {
  position: relative;
}

.skill-search {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #d4c5b0;
  margin-bottom: 12px;
}

.suggestions {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: white;
  border: 1px solid #d4c5b0;
  z-index: 10;
}

.suggestion {
  padding: 8px 12px;
  cursor: pointer;
  font-size: 13px;
  color: #5c4d3e;
}

.suggestion:hover {
  background: #efe6db;
}

.error {
  color: #8b2c1d;
  font-size: 12px;
  margin: 8px 0;
}
</style>