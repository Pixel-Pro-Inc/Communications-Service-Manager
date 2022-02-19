import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { SharedService } from '../_services/shared.service';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.css']
})
export class SigninComponent implements OnInit {

  signinForm: FormGroup;

  constructor(private fb: FormBuilder, private accountService: AccountService, public shared: SharedService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.signinForm = this.fb.group({
      accountId: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  signin(){
    this.accountService.login(this.signinForm.value);
  }

}
