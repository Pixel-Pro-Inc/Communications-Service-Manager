import { Component, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { SharedService } from '../_services/shared.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  user: any = {};
  model:any = {};

  code = `
  {
    "apiKey": "qFOL7kSwU4YWWqyZAIZ20bJfWhf78UopwNCGRUuvMh4=",
    "subject": "Message Subject",
    "message": "This is a message.",
    "recipients": [
        "76199359",
        "sebakilebernard@gmail.com",
        "76181741"
    ],
  }
  `;

  logs: any = {};
  constructor(private shared: SharedService, private messageService:MessageService) { }

  constructor(public shared: SharedService) { }

  ngOnInit(): void {
    this.user = this.shared.getUser();
    console.log("Whats the api key doing in the code yewo, Honest question, no judgement");
  }

  send() {
    this.messageService.SendMessage(this.model);
  }
}
