import { Injectable } from '@angular/core';
import { SharedService } from './shared.service';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  constructor(private shared: SharedService) { }

  Start(model: any){
    this.shared.busyService.busy('Starting your live chat server...');
    return this.shared.http.post(this.shared.baseUrl + 'livechat/start', model);
  }
}
