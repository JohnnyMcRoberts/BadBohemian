import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { UserAddRequest, UserAddResponse, UserLoginRequest, UserLoginResponse } from './../Models/User';

const httpOptions =
{
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()
export class UserLoginService
{
  constructor(private http: HttpClient)
  {
    this.addUserLoginUrl = 'api/Users/';
    this.requestUserLoginUrl = 'api/UserLogin/';
  }

  public addUserLoginUrl: string;
  public addUserLoginResponse: any;

  async getAsyncUserAdd(request: UserAddRequest)
  {
    this.addUserLoginResponse =
      await this.http.post<UserAddResponse>(
      this.addUserLoginUrl, request, httpOptions
      ).toPromise();

    console.log('No issues, waiting until promise is resolved...');
  }


  public requestUserLoginUrl: string;
  public userLoginResponse: any;

  async asyncUserLogin(request: UserLoginRequest)
  {
    this.userLoginResponse =
      await this.http.post<UserLoginResponse>(
        this.requestUserLoginUrl, request, httpOptions
      ).toPromise();

    console.log('asyncUserLogin: No issues, waiting until promise is resolved...');
  }

}
