import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import
{
     UserAddRequest,
     UserAddResponse,
     UserVerifyRequest,
     UserVerifyResponse,
     UserLoginRequest,
     UserLoginResponse

} from './../Models/users';
import { environment } from '../../environments/environment';

const httpOptions =
{
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()
export class UserLoginService
{
    constructor(private http: HttpClient) {

        let baseUrl = environment.baseUrl;

        //this.baseUsersUrl = 'api/Users/';
        this.baseUsersUrl = baseUrl + 'users/';
        this.addUserLoginUrl = this.baseUsersUrl + 'AddNewUser/';
        this.addUserVerifyUrl = this.baseUsersUrl + 'VerifyNewUser/';
        //this.requestUserLoginUrl = 'api/UserLogin/';
        this.requestUserLoginUrl = baseUrl + 'userlogin/';
    }

    public baseUsersUrl: string;
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

    public addUserVerifyUrl: string;
    public addUserVerifyResponse: any;

    async getAsyncUserVerify(request: UserVerifyRequest)
    {
        this.addUserVerifyResponse =
            await this.http.post<UserVerifyResponse>(
                this.addUserVerifyUrl, request, httpOptions
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
