import { Injectable } from '@angular/core';
import { Message } from '../_models/message';
import { SharedService } from './shared.service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private shared:SharedService) { }

  SendMessage(CreateMessageDto:any){
    
    if(CreateMessageDto.WhatsappOnly)
    {
      this.shared.http.post<Message>(this.shared.baseUrl + 'communications/sendwhatsapp', CreateMessageDto).subscribe(
        response=>{
       console.log(response);
     },
       error => {
         console.log(error);
       }
      );
    }
    if(!CreateMessageDto.WhatsappOnly)
    {
      this.shared.http.post<Message>(this.shared.baseUrl + 'communications/sendsms', CreateMessageDto).subscribe(
        response=>{
       console.log(response);
     },
       error => {
         console.log(error);
       }
      );
      this.shared.http.post<Message>(this.shared.baseUrl + 'communications/sendemail', CreateMessageDto).subscribe(
        response=>{
       console.log(response);
     },
       error => {
         console.log(error);
       }
      );
      this.shared.http.post<Message>(this.shared.baseUrl + 'communications/sendwhatsapp', CreateMessageDto).subscribe(
        response=>{
       console.log(response);
     },
       error => {
         console.log(error);
       }
      );
    }
     

  }
}
