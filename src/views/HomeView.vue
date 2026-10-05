
<script setup>
import axios from 'axios';
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import md5 from 'md5';
const router = useRouter();
const activeTab = ref('login'); 
const regName = ref('');
const regSurname = ref('');
const regPatronymic = ref('');
const regEmail = ref('');
const regPassword = ref('');
const regConfirmPassword = ref('');
const regError = ref('');
const loginEmail = ref('');
const loginPassword = ref('');
const loginError = ref('');
const twoFactorCode = ref('');
const show2FA = ref(false);
const tempToken = ref('');

const socialLogin = (provider) => {
  alert(`Заглушка: вход через ${provider}`);
};

const register = async () => {
  regError.value = '';
  
  if (regPassword.value.length < 8) {
    regError.value = 'Пароль должен быть больше 8 символов';
    return;
  }
  
  if (regPassword.value !== regConfirmPassword.value) {
    regError.value = 'Пароли не совпадают';
    return;
  }
  
  try {
    const response = await axios.post('http://localhost:5015/User/auth/register', {
      nameUser: regName.value,
      surnameUser: regSurname.value,
      patronymicUser: regPatronymic.value,
      emailUser: regEmail.value,
      passwordUser: regPassword.value,
      roleUser: 'user',
      dateCreate: new Date().toISOString()
    });
    
    if (response.status === 200 || response.status === 201) {
      alert('Регистрация успешна! Теперь войдите');
      activeTab.value = 'login';
      regName.value = '';
      regSurname.value = '';
      regPatronymic.value = '';
      regEmail.value = '';
      regPassword.value = '';
      regConfirmPassword.value = '';
    }
  } catch (err) {
    regError.value = err.response?.data || 'Ошибка регистрации';
  }
};

const login = async () => {
  loginError.value = '';
  
  try 
  {
    const response = await axios.post(`http://localhost:5015/User/auth/login/${loginEmail.value}/${md5(loginPassword.value)}`);
    
    if (response.status === 201) {
      const data = response.data;
      const encryptedToken = btoa(data.token);
      localStorage.setItem('auth_token', encryptedToken);
      localStorage.setItem('user_id', data.user.idUser);
      
        router.push('/dashboard');
      
    }
  } catch (err) {
    if (err.response?.status === 404) {
      loginError.value = 'Неверный email или пароль';
    } else {
      loginError.value = 'Ошибка входа';
    }
  }
};
const forgot = async() =>{
  try{
    const response = await axios.post(`http://localhost:5015/User/auth/forgot/${loginEmail.value}`)
    if(response.status==200){
      const data = response.data;
      alert(data)
    }
  }
  catch(err){
    alert(err)
  }
}

</script>
<template>
  <div class="container">
    <div class="tabs">
      <button @click="activeTab='login'" :class="{active: activeTab==='login'}">Войти</button>
      <button @click="activeTab='register'" :class="{active: activeTab==='register'}">Регистрация</button>
    </div>

    <div v-if="activeTab==='login' && !show2FA">
      <h2>Вход в аккаунт</h2>

      <div class="field">
        <p>Email</p>
        <input type="email" v-model="loginEmail" placeholder="ivan@example.com"/>
      </div>

      <div class="field">
        <p>Пароль</p>
        <input type="password" v-model="loginPassword"/>
      </div>

      <p v-if="loginError" class="error">{{ loginError }}</p>

      <button @click="login">Войти</button>
      <button @click="forgot">Забыли пароль</button>

      <div class="social">
        <p>Или войдите через</p>
        <button @click="socialLogin('google')">Google</button>
        <button @click="socialLogin('github')">GitHub</button>
      </div>
    </div>

    <div v-if="activeTab==='register'">
      <h2>Создание аккаунта</h2>

      <div class="field">
        <p>Имя</p>
        <input v-model="regName"/>
      </div>

      <div class="field">
        <p>Фамилия</p>
        <input v-model="regSurname"/>
      </div>

      <div class="field">
        <p>Отчество</p>
        <input v-model="regPatronymic"/>
      </div>

      <div class="field">
        <p>Email</p>
        <input type="email" v-model="regEmail"/>
      </div>

      <div class="field">
        <p>Пароль (более 8 символов)</p>
        <input type="password" v-model="regPassword"/>
      </div>

      <div class="field">
        <p>Подтверждение пароля</p>
        <input type="password" v-model="regConfirmPassword"/>
      </div>

      <p v-if="regError" class="error">{{ regError }}</p>

      <button @click="register">Зарегистрироваться</button>

      <div class="social">
        <p>Или зарегистрируйтесь через</p>
        <button @click="socialLogin('google')">Google</button>
        <button @click="socialLogin('github')">GitHub</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      activeTab: 'login',
      show2FA: false,
      loginEmail: '',
      loginPassword: '',
      loginError: '',
      regName: '',
      regSurname: '',
      regPatronymic: '',
      regEmail: '',
      regPassword: '',
      regConfirmPassword: '',
      regError: ''
    }
  },
  methods: {
    login() {},
    forgot() {},
    socialLogin(provider) {},
    register() {}
  }
}
</script>

<style scoped>
.container {
  max-width: 460px;
  margin: 50px auto;
  padding: 32px 28px;
  background: #faf8f5;
  border-radius: 0;
  box-shadow: none;
}

.tabs {
  display: flex;
  gap: 0;
  margin-bottom: 32px;
  border-bottom: 1px solid #d4c5b0;
}

.tabs button {
  flex: 1;
  padding: 12px 16px;
  background: none;
  border: none;
  font-size: 16px;
  font-weight: normal;
  cursor: pointer;
  color: #8b7a6b;
  border-bottom: 2px solid transparent;
  transition: none;
}

.tabs button.active {
  color: #8b2c1d;
  border-bottom-color: #8b2c1d;
}

h2 {
  font-size: 20px;
  font-weight: normal;
  margin-bottom: 24px;
  color: #4a3b2c;
}

.field {
  margin-bottom: 20px;
}

.field p {
  margin-bottom: 8px;
  font-weight: normal;
  font-size: 14px;
  color: #5c4d3e;
}

.field input {
  width: 100%;
  padding: 12px;
  border: 1px solid #d4c5b0;
  background: white;
  font-size: 14px;
  box-sizing: border-box;
}

.field input:focus {
  outline: none;
  border-color: #8b2c1d;
}

button {
  width: 100%;
  padding: 12px;
  background: #8b2c1d;
  border: none;
  color: white;
  font-size: 15px;
  cursor: pointer;
  margin-top: 8px;
}

button:hover {
  background: #6e2216;
}

.social {
  margin-top: 24px;
  text-align: center;
}

.social p {
  font-size: 13px;
  color: #8b7a6b;
  margin-bottom: 12px;
}

.social button {
  width: auto;
  margin: 5px;
  padding: 8px 20px;
  background: transparent;
  border: 1px solid #d4c5b0;
  color: #5c4d3e;
}

.social button:hover {
  background: #efe6db;
  border-color: #8b2c1d;
}

.error {
  color: #8b2c1d;
  font-size: 13px;
  margin-top: -8px;
  margin-bottom: 12px;
}
</style>