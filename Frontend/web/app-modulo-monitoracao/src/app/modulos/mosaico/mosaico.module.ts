import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MosaicoRoutingModule } from './mosaico-routing.module';
import { MosaicoComponent } from './mosaico.component';


@NgModule({
  declarations: [
    MosaicoComponent
  ],
  imports: [
    CommonModule,
    MosaicoRoutingModule
  ]
})
export class MosaicoModule { }
