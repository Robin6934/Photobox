import { computed, ref } from "vue";
import { defineStore } from "pinia";
import { ApiException, UserMeResponse, LoginRequest } from "@/OpenApi/Client";
import { apiClient } from "@/services/api";
import { clearTokens, setTokens } from "@/services/tokenStore";

export const useAuthStore = defineStore("auth", () => {
  const currentUser = ref<UserMeResponse | null>(null);
  const initialized = ref(false);
  const loading = ref(false);

  const isAuthenticated = computed(() => currentUser.value !== null);

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
    if (initialized.value) return;
    if (initializePromise) return initializePromise;

    initializePromise = refreshCurrentUser().finally(() => {
      initializePromise = null;
    });

    return initializePromise;
  };

  const login = async (
    email: string,
    password: string,
    twoFactorCode?: string,
    twoFactorRecoveryCode?: string,
  ) => {
    loading.value = true;
    try {
      const response = await apiClient.postApiLogin(
        false,
        false,
        new LoginRequest({ email, password, twoFactorCode, twoFactorRecoveryCode }),
      );
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
    try {
      await apiClient.postApiLogout();
    } catch {
      // network error or expired token — still clear locally
    } finally {
      clearSession();
      loading.value = false;
    }
  };

  return { currentUser, initialized, loading, isAuthenticated, initializeAuth, refreshCurrentUser, login, logout, clearSession };
});

export const handleUnauthorized = () => {
  useAuthStore().clearSession();
};
