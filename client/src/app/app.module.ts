import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { ToastrModule } from 'ngx-toastr';
import { SignupComponent } from './signup/signup.component';
import { SigninComponent } from './signin/signin.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { NavComponent } from './nav/nav.component';
import {HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { NgxSpinnerModule } from 'ngx-spinner';

@NgModule({
  declarations: [
    AppComponent,
    SignupComponent,
    SigninComponent,
    DashboardComponent,
    NavComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    NgxSpinnerModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
