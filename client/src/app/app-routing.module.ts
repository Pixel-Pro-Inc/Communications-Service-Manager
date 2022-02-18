import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { SigninComponent } from './signin/signin.component';
import { SignupComponent } from './signup/signup.component';
import { LoggedInGuard } from './_guards/logged-in.guard';

const routes: Routes = [
  {path: '', component:DashboardComponent, canActivate:[LoggedInGuard]},
  {path: 'signup', component:SignupComponent},
  {path: 'signin', component:SigninComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
