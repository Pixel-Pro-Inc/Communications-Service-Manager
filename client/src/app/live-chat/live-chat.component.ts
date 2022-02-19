import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Client as ConversationsClient, Message } from "@twilio/conversations";
import { ChatService } from '../_services/chat.service';
import { SharedService } from '../_services/shared.service';

@Component({
  selector: 'app-live-chat',
  templateUrl: './live-chat.component.html',
  styleUrls: ['./live-chat.component.css']
})
export class LiveChatComponent implements OnInit {
  conversationsclient: ConversationsClient;
  conversationSid: string;
  messages: Message[] = [];

  conversationStarted = false;

  startChatForm: FormGroup;
  msgForm: FormGroup;

  constructor(private fb: FormBuilder, public shared: SharedService, private chatService: ChatService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.startChatForm = this.fb.group({
      number: ['', [Validators.required]]
    });

    this.msgForm = this.fb.group({
      message: ['', [Validators.required]]
    });
  }

  startLiveChat(){
    let model: any = {};
    this.messages = [];
    model.conversationName = 'My First Convo';
    model.participants = (this.startChatForm.controls.number.value.toString());
    model.apiKey = this.shared.getUser().apiKey;

    console.log(model);

    this.chatService.Start(model).subscribe(arg => {
      console.log(arg);

      let mod: any = {};

      mod = arg;

      this.conversationSid = mod.conversationSid;
      this.initChat(mod.token);

      this.shared.busyService.idle();
    },
    error => {
      console.log(error);
    });
  }

  async sendMessage(msg: string){
    (await this.conversationsclient.getConversationBySid(this.conversationSid)).sendMessage(msg);
  }

  async initChat(twilio_token: string){
    let client = await ConversationsClient.create(twilio_token);
    this.conversationsclient = client;

    console.log(client);

    this.conversationsclient.on("connectionStateChanged", (state) => {
      if (state === "connecting") {
         console.log('connectionStateChanged', state);
      }
      if (state === "connected") {
         console.log('connectionStateChanged', state);
      }
      if (state === "disconnecting") {
         console.log('connectionStateChanged', state);
      }
      if (state === "disconnected") {
        console.log('connectionStateChanged', state);
      }
      if (state === "denied") {
         console.log('connectionStateChanged', state);
      }
    });

    this.sendMessage('Hello from ' + this.shared.getUser().organizationName + ', you can respond to this message to start a live chat with us.');
    this.conversationStarted = true;

    this.conversationsclient.on("messageAdded", (msg) => {
      this.messages.push(msg);
    });
  }
}