import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { CamerasRoutingModule } from './cameras-routing.module';
import { CamerasComponent } from './cameras.component';


@NgModule({
  declarations: [
    CamerasComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    CamerasRoutingModule
  ]
})
export class CamerasModule { }
