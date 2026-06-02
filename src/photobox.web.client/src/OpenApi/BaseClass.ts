import { getAccessToken } from '@/services/tokenStore';

export class BaseClass {

  protected transformOptions = async (options: RequestInit): Promise<RequestInit> => {
    const token = getAccessToken();
    const headers = new Headers(options.headers as HeadersInit);
    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }
    return Promise.resolve({
      ...options,
      headers,
    });
  };
}
