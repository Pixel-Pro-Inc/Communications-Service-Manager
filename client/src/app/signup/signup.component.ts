import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { SharedService } from '../_services/shared.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  signupForm: FormGroup;

  constructor(private fb: FormBuilder, private accountService: AccountService, public shared: SharedService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.signupForm = this.fb.group({
      organization: ['', [Validators.required, this.containsNumbers()]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });

    this.signupForm.controls.password.valueChanges.subscribe(() => {
      this.signupForm.controls.confirmPassword.updateValueAndValidity();
    });
  }

  containsNumbers(): ValidatorFn{
    return (control: AbstractControl) => {
      return (/^[A-Za-z]+$/).test(control?.value)? null : control?.value == ''? null : {hasNumbers: true};
    };
  }

  isNumber(): ValidatorFn{
    return (control: AbstractControl) => {
      return isNaN(control?.value) ? {hasLetters: true} : null;
    };
  }

  matchValues(macthTo: string): ValidatorFn{
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[macthTo].value ? null : {isMatching: true};
    };
  }

  createAccount(){
    this.accountService.signup(this.signupForm.value);
  }

}
