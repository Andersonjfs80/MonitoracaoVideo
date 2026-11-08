import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [{ path: 'mosaico', loadChildren: () => import('./modulos/mosaico/mosaico.module').then(m => m.MosaicoModule) }, { path: 'cameras', loadChildren: () => import('./modulos/cameras/cameras.module').then(m => m.CamerasModule) }, { path: 'timeline', loadChildren: () => import('./modulos/timeline/timeline.module').then(m => m.TimelineModule) }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
