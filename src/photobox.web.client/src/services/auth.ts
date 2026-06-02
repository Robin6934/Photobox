import { computed, readonly, ref } from "vue";
import { ApiException, ApplicationUser, LoginRequest } from "@/OpenApi/Client";
import { apiClient } from "@/services/api";
import { clearTokens, setTokens } from "@/services/tokenStore";

const currentUser = ref<ApplicationUser | null>(null);
const initialized = ref(false);
const loading = ref(false);

let initializePromise: Promise<void> | null = null;

const clearSession = () => {
  currentUser.value = null;
  initialized.value = true;
  clearTokens();
};

const refreshCurrentUser = async () => {
  try {
    currentUser.value = await apiClient.getUsersMe();
  } catch (error) {
    if (error instanceof ApiException && (error.status === 401 || error.status === 403)) {
      currentUser.value = null;
      return;
    }

    throw error;
  } finally {
    initialized.value = true;
  }
};

const initializeAuth = async () => {
  if (initialized.value) {
    return;
  }

  if (initializePromise) {
    return initializePromise;
  }

  initializePromise = refreshCurrentUser().finally(() => {
    initializePromise = null;
  });

  return initializePromise;
};

const login = async (email: string, password: string, _rememberMe: boolean) => {
  loading.value = true;

  try {
    const response = await apiClient.postApiLogin(false, false, new LoginRequest({ email, password }));
    if (response.accessToken && response.refreshToken) {
      setTokens(response.accessToken, response.refreshToken);
    }
    await refreshCurrentUser();
  } finally {
    loading.value = false;
  }
};

const logout = async () => {
  loading.value = true;
  clearSession();
  loading.value = false;
};

export const handleUnauthorized = () => {
  clearSession();
};

export const useAuth = () => ({
  currentUser: readonly(currentUser),
  initialized: readonly(initialized),
  isAuthenticated: computed(() => currentUser.value !== null),
  loading: readonly(loading),
  initializeAuth,
  refreshCurrentUser,
  login,
  logout,
});