import { Component } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators, ValidatorFn, ValidationErrors, AbstractControl } from '@angular/forms';

import {
    UserAddRequest,
    UserAddResponse,
    UserVerifyRequest,
    UserVerifyResponse,
    UserLoginRequest,
    UserLoginResponse,
    UserLogin
} from './../../../Models/users';

import { BooksDataService } from './../../../Services/books-data.service';
import { CurrentLoginService } from './../../../Services/current-login.service';
import { UserLoginService } from './../../../Services/user-login.service';

@Component({
    selector: 'app-main-user-login-page',
    templateUrl: './main-user-login-page.component.html',
    styleUrls: ['./main-user-login-page.component.scss']
})
/** MainUserLoginPage component*/
export class MainUserLoginPageComponent {
    /** MainUserLoginPage ctor */
    constructor(
        private formBuilder: FormBuilder,
        public currentLoginService: CurrentLoginService,
        private booksDataService: BooksDataService,
        private userLoginService: UserLoginService) {

    }

    //#region OnInit Implementation

    ngOnInit() {
        this.setupForGroups();
    }

    //#endregion

    //#region Form Group Data setup

    minNameLength = 4;
    minPasswordLength = 8;

    public newUserFormGroup: FormGroup | any;
    public existingUserFormGroup: FormGroup | any;

    public setupForGroups(): void {
        this.newUserFormGroup =
            this.formBuilder.group(
                {
                    newUserName: ['', [Validators.required, Validators.minLength(this.minNameLength)]],
                    newUserEmail: ['', [Validators.required, Validators.email]],
                    newUserDescription: [''],
                    password: ['', [Validators.required, Validators.minLength(this.minPasswordLength)]],
                    password2: ['', [Validators.required]]
                },
                { validator: passwordMatchValidator });

        this.existingUserFormGroup = this.formBuilder.group(
            {
                existingUserName: ['', [Validators.required]],
                existingUserPassword: ['', [Validators.required]]
            });
    }

    public clearFormGroupControl(formGroup: FormGroup, controlName: string): void {
        (formGroup.get(controlName) as AbstractControl).clearValidators();
        (formGroup.get(controlName) as AbstractControl).reset();
        (formGroup.get(controlName) as AbstractControl).markAsPristine();
        (formGroup.get(controlName) as AbstractControl).markAsUntouched();
        (formGroup.get(controlName) as AbstractControl).updateValueAndValidity();
    }

    //#endregion

    //#region New User Form data and processing

    public addUserLoginSuccessString: string = '';
    public addUserLoginErrorString: string = '';
    public addUserId: string = '';
    public newUserLoginValidating: boolean = false;

    /* Shorthands for form controls (used from within template) */
    get newUserName() { return this.newUserFormGroup.get('newUserName'); }
    get newUserEmail() { return this.newUserFormGroup.get('newUserEmail'); }
    get newUserDescription() { return this.newUserFormGroup.get('newUserDescription'); }
    get password() { return this.newUserFormGroup.get('password'); }
    get password2() { return this.newUserFormGroup.get('password2'); }

    /* Called on each input in either password field */
    onNewPasswordInput() {
        this.addUserLoginErrorString = '';
        this.addUserLoginSuccessString = '';
        if (this.newUserFormGroup.hasError('passwordMismatch')) {
            this.password2.setErrors([{ 'passwordMismatch': true }]);
        }
        else {
            this.password2.setErrors(null);
        }
    }

    onNewUserNameInput() {
        this.addUserLoginErrorString = '';
        this.addUserLoginSuccessString = '';
    }

    onNewUserEmailInput() {
        // check if unique, maybe later
        this.addUserLoginErrorString = '';
        this.addUserLoginSuccessString = '';
    }

    onNewUserDescriptionInput() {
        // check if unique????
        this.addUserLoginErrorString = '';
        this.addUserLoginSuccessString = '';
    }

    public async onNewUserSubmitted() {
        console.log('onNewUserSubmitted -> newUserName : ', this.newUserFormGroup.value.newUserName);
        this.newUserLoginValidating = false;

        const addUserReq: UserAddRequest =
            new UserAddRequest(this.newUserFormGroup.value.newUserName,
                this.newUserFormGroup.value.password,
                this.newUserFormGroup.value.newUserDescription,
                this.newUserFormGroup.value.newUserEmail);

        await this.userLoginService.getAsyncUserAdd(addUserReq);

        const resp = this.userLoginService.addUserLoginResponse;

        if (resp == undefined) {
            console.log("Error in response");
        }
        else {
            console.log("Response OK");

            const addResponse: UserAddResponse = UserAddResponse.fromData(resp);

            if (addResponse.errorCode === 0) {
                this.addUserId = addResponse.userId;
                this.addUserLoginSuccessString = "Added New User Id: " + this.addUserId;
                this.addUserLoginErrorString = '';

                const userLogin: UserLogin =
                    new UserLogin(
                        this.newUserFormGroup.value.newUserName,
                        this.newUserFormGroup.value.newUserDescription,
                        this.newUserFormGroup.value.newUserEmail,
                        addResponse.userId);

                this.currentLoginService.login(userLogin);

                this.newUserFormGroup.reset();
                this.newUserFormGroup.clearValidators();
                this.newUserFormGroup.markAsPristine();
                this.clearFormGroupControl(this.newUserFormGroup, 'newUserName');
                this.clearFormGroupControl(this.newUserFormGroup, 'newUserEmail');
                this.clearFormGroupControl(this.newUserFormGroup, 'newUserDescription');
                this.clearFormGroupControl(this.newUserFormGroup, 'password');
                this.clearFormGroupControl(this.newUserFormGroup, 'password2');

                this.newUserFormGroup.markAsPristine();
                this.newUserFormGroup.markAsUntouched();
                this.newUserFormGroup.updateValueAndValidity();

                console.log("should have a cleared form....");

                this.newUserLoginValidating = true;
            }
            else {
                this.addUserLoginSuccessString = '';
                this.addUserLoginErrorString = addResponse.failReason;
            }
        }
    }

    //#endregion

    //#region New User Confirmation

    confirmationCodeForm: FormControl = new FormControl();

    confirmationCodeValue: string = '';

    verificationError: string | any = undefined;
    verificationSuccess: boolean = false;

    async onSubmitConfirmationCode() {
        this.confirmationCodeValue = this.confirmationCodeForm.value as string;
        console.log("got a new confirmation code: " + this.confirmationCodeValue);

        // make up the very request
        const verifyUserReq: UserVerifyRequest =
            new UserVerifyRequest(this.addUserId, this.confirmationCodeValue);

        await this.userLoginService.getAsyncUserVerify(verifyUserReq);

        const resp = this.userLoginService.addUserVerifyResponse;

        if (resp == undefined) {
            console.log("Error in response");
        }
        else {
            console.log("Response OK");

            const verifyResponse: UserVerifyResponse = UserVerifyResponse.fromData(resp);

            if (verifyResponse.errorCode === 0) {
                // set as the current log in 
                const userLogin: UserLogin =
                    new UserLogin(
                        verifyResponse.name,
                        verifyResponse.description,
                        verifyResponse.email,
                        verifyResponse.userId);

                this.currentLoginService.login(userLogin);

                this.verificationSuccess = true;
            }
            else {
                this.verificationError = verifyResponse.failReason;
            }
        }
    }

    //#endregion

    //#region Existing User Login Form data and processing

    public existingUserLoginSuccessString = '';
    public existingUserLoginErrorString = '';
    public existingUserLoginInProgressName = '';

    /* Shorthands for form controls (used from within template) */
    get existingUserName() {
        return this.existingUserFormGroup.get('existingUserName');
    }

    get existingUserPassword() {
        return this.existingUserFormGroup.get('existingUserPassword');
    }

    public onExistingUserNameInput() {
        this.existingUserLoginErrorString = '';
        this.existingUserLoginSuccessString = '';
    }

    public onExistingPasswordInput() {
        this.existingUserLoginErrorString = '';
        this.existingUserLoginSuccessString = '';
    }

    public async onExistingUserSubmitted() {
        console.log('onExistingUserSubmitted -> Name : ', this.existingUserFormGroup.value.existingUserName);

        this.existingUserLoginInProgressName = this.existingUserFormGroup.value.existingUserName;

        const userLoginReq: UserLoginRequest =
            new UserLoginRequest(
                this.existingUserFormGroup.value.existingUserName,
                this.existingUserFormGroup.value.existingUserPassword);

        await this.userLoginService.asyncUserLogin(userLoginReq);

        const resp = this.userLoginService.userLoginResponse;
        this.existingUserLoginInProgressName = '';

        if (resp == undefined) {
            console.log("Error in response");
        }
        else {
            console.log("Response OK");

            const loginResponse: UserLoginResponse = UserLoginResponse.fromData(resp);

            if (loginResponse.errorCode === 0) {
                this.existingUserLoginSuccessString = "Logged in successfully with User Id: " + loginResponse.userId;
                this.existingUserLoginErrorString = '';

                const userLogin: UserLogin =
                    new UserLogin(
                        loginResponse.name,
                        loginResponse.description,
                        loginResponse.email,
                        loginResponse.userId);

                this.currentLoginService.login(userLogin);

                this.existingUserFormGroup.reset();
                this.existingUserFormGroup.clearValidators();
                this.existingUserFormGroup.markAsPristine();
                this.clearFormGroupControl(this.existingUserFormGroup, 'existingUserName');
                this.clearFormGroupControl(this.existingUserFormGroup, 'existingUserPassword');
            }
            else {
                this.existingUserLoginSuccessString = '';
                this.existingUserLoginErrorString = loginResponse.failReason;
            }
        }
    }

    public onExistingUserReset() {
        this.existingUserLoginSuccessString = '';
    }

    public onSetAsDefaultUser() {

        console.log('onSetAsDefaultUser -> Name : ', this.currentLoginService.name);

        this.booksDataService.getAsDefaultUser(this.currentLoginService.userId).then(
            () => {
                console.log('Completed : onSetAsDefaultUser for ', this.currentLoginService.name);
            });

    }

    //#endregion
}


/** A hero's name can't match the given regular expression */
export function forbiddenNameValidator(nameRe: RegExp): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const forbidden = nameRe.test(control.value);
        return forbidden ? { forbiddenName: { value: control.value } } : null;
    };
}

export function passwordMatchValidator(): ValidatorFn
{
    return (control: AbstractControl): ValidationErrors | null =>
    {
        const formGroup: FormGroup = control as FormGroup;

        const matching: boolean =
            ((formGroup.get('password') as AbstractControl).value
                === (formGroup.get('password2') as AbstractControl).value);

        return !matching ? { passwordMismatch: { value: true } } : null;
    };
}

//export function passwordMatchValidator(nameRe: RegExp): ValidatorFn {
//    return (formGroup: FormGroup): ValidationErrors | null => {
//        if ((formGroup.get('password') as AbstractControl).value ===
//            (formGroup.get('password2') as AbstractControl).value) {
//            return null;
//        }

//        return { passwordMismatch: true };

//    };
//}
