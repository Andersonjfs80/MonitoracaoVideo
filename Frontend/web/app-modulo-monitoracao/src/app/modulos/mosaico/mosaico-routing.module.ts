import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MosaicoComponent } from './mosaico.component';

const routes: Routes = [{ path: '', component: MosaicoComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MosaicoRoutingModule { }
