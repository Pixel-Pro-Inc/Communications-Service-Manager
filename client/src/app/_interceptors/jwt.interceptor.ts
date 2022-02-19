import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { SharedService } from '../_services/shared.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private shared: SharedService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if(this.shared.getUser() != null){
      let token = this.shared.getUser().token;
      request = request.clone({
        setHeaders:{
          Authorization: `Bearer ${token}`
        }
      });
    }    

    return next.handle(request);
  }
}