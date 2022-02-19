import { Injectable } from '@angular/core';
import { SharedService } from './shared.service';

@Injectable({
  providedIn: 'root'
})
export class DashService {

  constructor(private shared: SharedService) { }

  getLogs(){
    return this.shared.http.get(this.shared.baseUrl + 'dashboard/logs');
  }
}
