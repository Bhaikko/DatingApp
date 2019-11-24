import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {

  private values: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    // Initialised after constructor but before the component is initialised
    this.getValues();
  }

  getValues () {
    this.http.get("http://localhost:5000/api/values").subscribe(response => {
      this.values = response;
    }, err => {
      console.log(err);
    });
  }

}
