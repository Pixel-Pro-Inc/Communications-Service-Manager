import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { SharedService } from '../_services/shared.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  constructor(public shared: SharedService, private account: AccountService) { }

  ngOnInit(): void {
  }

  logout(){
    this.account.logout();
  }
}
