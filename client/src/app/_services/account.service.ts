import { Injectable } from '@angular/core';
import { SharedService } from './shared.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private shared: SharedService) {
  }

  signup(model: any){
    console.log(model);
    this.shared.busyService.busy('Creating your account...');
    this.shared.http.post(this.shared.baseUrl + 'account/signup', model).subscribe(
      response =>{
        console.log(response)
        this.shared.setUser(response);//Logs a user in
        this.shared.busyService.idle();
        this.shared.router.navigateByUrl('/');
      },
      error => {
        this.shared.busyService.idle();
        this.shared.toastr.error(error.error);
      }
    );
  }

  login(model: any){
    this.shared.busyService.busy('Signing you in');
    this.shared.http.post(this.shared.baseUrl + 'account/login', model).subscribe(
      response =>{
        this.shared.setUser(response);//Logs a user in
        this.shared.router.navigateByUrl('/');
        this.shared.busyService.idle();
      },
      error => {
        this.shared.busyService.idle();
        this.shared.toastr.error(error.error);
      }
    );
  }

  logout(){
    this.shared.removeUser();
  }
}
