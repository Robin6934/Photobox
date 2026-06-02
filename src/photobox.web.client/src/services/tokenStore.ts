const ACCESS_KEY = 'pb_access_token';
const REFRESH_KEY = 'pb_refresh_token';

let _accessToken: string | null = localStorage.getItem(ACCESS_KEY);
let _refreshToken: string | null = localStorage.getItem(REFRESH_KEY);

export const getAccessToken = (): string | null => _accessToken;
export const getRefreshToken = (): string | null => _refreshToken;

export const setTokens = (accessToken: string, refreshToken: string): void => {
  _accessToken = accessToken;
  _refreshToken = refreshToken;
  localStorage.setItem(ACCESS_KEY, accessToken);
  localStorage.setItem(REFRESH_KEY, refreshToken);
};

export const clearTokens = (): void => {
  _accessToken = null;
  _refreshToken = null;
  localStorage.removeItem(ACCESS_KEY);
  localStorage.removeItem(REFRESH_KEY);
};