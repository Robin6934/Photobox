export class BaseClass {

  protected transformOptions = async (options: RequestInit): Promise<RequestInit> => {
    let token = localStorage.getItem("JWT"); // your custom logic to get the token

    options.headers = {
      ...options.headers,
      Authorization: 'Bearer ' + token,
    };
    return Promise.resolve(options);
  };
}
