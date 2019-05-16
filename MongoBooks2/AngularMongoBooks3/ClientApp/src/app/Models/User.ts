export interface IUserAddRequest
{
    name: string;
    password: string;
    description: string;
    email: string;
    imageUri: string;
};

export class UserAddRequest implements IUserAddRequest
{
    static fromData(data: IUserAddRequest) {
        return new this(
            data.name,
            data.password,
            data.description,
            data.email,
            data.imageUri);
    }

    constructor(
        public name: string = "",
        public password: string = "",
        public description: string = "",
        public email: string = "",
        public imageUri: string = "")
    {
    }
};

export interface IUserAddResponse {
    name: string;
    errorCode: number;
    failReason: string;
    userId: string;
};

export class UserAddResponse implements IUserAddResponse
{
    static fromData(data: IUserAddResponse)
    {
        return new this(
            data.name,
            data.errorCode,
            data.failReason,
            data.userId);
    }

    constructor(
        public name: string = "",
        public errorCode: number = -1,
        public failReason: string = "",
        public userId: string = "")
    {
    }
};

export interface IUserLogin
{
    name: string;
    description: string;
    email: string;
    userId: string;
};

export class UserLogin implements IUserLogin
{
    static fromData(data: IUserLogin)
    {
        return new this(
            data.name,
            data.description,
            data.email,
            data.userId);
    }

    constructor(
        public name: string = "",
        public description: string = "",
        public email: string = "",
        public userId: string = "")
    {
    }
};

export interface IUserLoginRequest
{
    name: string;
    password: string;
};

export class UserLoginRequest implements IUserLoginRequest
{
    static fromData(data: IUserLoginRequest) {
        return new this(
            data.name,
            data.password);
    }

    constructor(
        public name: string = "",
        public password: string = "") {
    }
};


export interface IUserLoginResponse
{
    name: string;
    errorCode: number;
    failReason: string;
    userId: string;
    description: string;
    email: string;
};

export class UserLoginResponse implements IUserLoginResponse {

    static fromData(data: IUserLoginResponse) {
        return new this(
            data.name,
            data.errorCode,
            data.failReason,
            data.userId,
            data.description,
            data.email);
    }

    constructor(
        public name: string = "",
        public errorCode: number = 0,
        public failReason: string = "",
        public userId: string = "",
        public description: string = "",
        public email: string = "")
    {
    }
};



