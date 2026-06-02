import { Client, GalleryClient, PhotoBoxClient } from "@/OpenApi/Client";
import { handleUnauthorized } from "@/services/auth";
import { getAccessToken, getRefreshToken, setTokens } from "@/services/tokenStore";

let isRefreshing = false;
let refreshQueue: Array<(success: boolean) => void> = [];

const tryRefresh = async (): Promise<boolean> => {
  if (isRefreshing) {
    return new Promise(resolve => refreshQueue.push(resolve));
  }

  const refreshToken = getRefreshToken();
  if (!refreshToken) return false;

  isRefreshing = true;
  try {
    const response = await window.fetch('/api/refresh', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    });

    if (!response.ok) {
      refreshQueue.forEach(r => r(false));
      refreshQueue = [];
      return false;
    }

    const data = await response.json();
    if (data.accessToken && data.refreshToken) {
      setTokens(data.accessToken, data.refreshToken);
      refreshQueue.forEach(r => r(true));
      refreshQueue = [];
      return true;
    }

    refreshQueue.forEach(r => r(false));
    refreshQueue = [];
    return false;
  } catch {
    refreshQueue.forEach(r => r(false));
    refreshQueue = [];
    return false;
  } finally {
    isRefreshing = false;
  }
};

const httpClient = {
  fetch: async (url: RequestInfo, init?: RequestInit) => {
    const response = await window.fetch(url, {
      ...init,
      credentials: init?.credentials ?? "same-origin",
    });

    if (response.status === 401) {
      const refreshed = await tryRefresh();
      if (refreshed) {
        const token = getAccessToken();
        const headers = new Headers(init?.headers as HeadersInit);
        if (token) headers.set('Authorization', `Bearer ${token}`);
        const retry = await window.fetch(url, { ...init, headers, credentials: init?.credentials ?? "same-origin" });
        if (retry.status !== 401) return retry;
      }
      handleUnauthorized();
    }

    return response;
  },
};

export const apiClient = new Client(undefined, httpClient);
export const galleryClient = new GalleryClient(undefined, httpClient);
export const photoBoxClient = new PhotoBoxClient(undefined, httpClient);