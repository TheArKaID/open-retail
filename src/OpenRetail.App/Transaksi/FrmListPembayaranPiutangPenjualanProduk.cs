﻿/**
 * Copyright (C) 2017 Kamarudin (http://coding4ever.net/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * The latest version of this file can be found at https://github.com/rudi-krsoftware/open-retail
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenRetail.Model;
using OpenRetail.Bll.Api;
using OpenRetail.Bll.Service;
using OpenRetail.App.UI.Template;
using OpenRetail.App.Helper;
using Syncfusion.Windows.Forms.Grid;
using ConceptCave.WaitCursor;
using log4net;

namespace OpenRetail.App.Transaksi
{
    public partial class FrmListPembayaranPiutangPenjualanProduk : FrmListEmptyBody, IListener
    {
        private IPembayaranPiutangProdukBll _bll; // deklarasi objek business logic layer 
        private IList<PembayaranPiutangProduk> _listOfPembayaranPiutang = new List<PembayaranPiutangProduk>();
        private ILog _log;
        
        public FrmListPembayaranPiutangPenjualanProduk(string header)
            : base()
        {
            InitializeComponent();

            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;

            _log = MainProgram.log;
            _bll = new PembayaranPiutangProdukBll(_log);

            LoadData(filterRangeTanggal.TanggalMulai, filterRangeTanggal.TanggalSelesai);

            InitGridList();
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Tanggal", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Nota Pembayaran", Width = 120 });
            gridListProperties.Add(new GridListControlProperties { Header = "Customer", Width = 300 });
            gridListProperties.Add(new GridListControlProperties { Header = "Pembayaran", Width = 150 });
            gridListProperties.Add(new GridListControlProperties { Header = "Keterangan" });            

            GridListControlHelper.InitializeGridListControl<PembayaranPiutangProduk>(this.gridList, _listOfPembayaranPiutang, gridListProperties);

            if (_listOfPembayaranPiutang.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {

                if (_listOfPembayaranPiutang.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfPembayaranPiutang.Count)
                        {
                            var pembayaran = _listOfPembayaranPiutang[rowIndex];
                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = DateTimeHelper.DateToString(pembayaran.tanggal);
                                    break;

                                case 3:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = pembayaran.nota;
                                    break;

                                case 4:
                                    var customer = pembayaran.Customer;

                                    if (customer != null)
                                        e.Style.CellValue = customer.nama_customer;

                                    break;

                                case 5:
                                    var total = pembayaran.item_pembayaran_piutang.Sum(f => f.nominal);

                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(total);

                                    break;

                                case 6:
                                    e.Style.CellValue = pembayaran.keterangan;
                                    break;

                                default:
                                    break;
                            }

                            // we handled it, let the grid know
                            e.Handled = true;
                        }
                    }
                }
            };
        }

        private void LoadData()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfPembayaranPiutang = _bll.GetAll();
                GridListControlHelper.Refresh<PembayaranPiutangProduk>(this.gridList, _listOfPembayaranPiutang);
            }

            ResetButton();
        }

        private void LoadData(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfPembayaranPiutang = _bll.GetByTanggal(tanggalMulai, tanggalSelesai);
                GridListControlHelper.Refresh<PembayaranPiutangProduk>(this.gridList, _listOfPembayaranPiutang);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfPembayaranPiutang.Count > 0);
        }

        protected override void Tambah()
        {
            var frm = new FrmEntryPembayaranPiutangPenjualanProduk("Tambah Data " + this.Text, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Perbaiki()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var pembayaran = _listOfPembayaranPiutang[index];

            var frm = new FrmEntryPembayaranPiutangPenjualanProduk("Edit Data " + this.Text, pembayaran, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Hapus()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            var pembayaran = _listOfPembayaranPiutang[index];
            if (pembayaran.is_tunai)
            {
                MsgHelper.MsgWarning("Maaf pembayaran piutang penjualan tunai tidak bisa dihapus");
                return;
            }

            if (MsgHelper.MsgDelete())
            {                
                var result = _bll.Delete(pembayaran);
                if (result > 0)
                {
                    GridListControlHelper.RemoveObject<PembayaranPiutangProduk>(this.gridList, _listOfPembayaranPiutang, pembayaran);
                    ResetButton();
                }
                else
                    MsgHelper.MsgDeleteError();
            }
        }

        public void Ok(object sender, object data)
        {
            throw new NotImplementedException();
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            var pembayaran = (PembayaranPiutangProduk)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<PembayaranPiutangProduk>(this.gridList, _listOfPembayaranPiutang, pembayaran);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<PembayaranPiutangProduk>(this.gridList, _listOfPembayaranPiutang, pembayaran);
        }

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (btnPerbaiki.Enabled)
                Perbaiki();
        }

        private void filterRangeTanggal_BtnTampilkanClicked(object sender, EventArgs e)
        {
            var tanggalMulai = filterRangeTanggal.TanggalMulai;
            var tanggalSelesai = filterRangeTanggal.TanggalSelesai;

            if (!DateTimeHelper.IsValidRangeTanggal(tanggalMulai, tanggalSelesai))
            {
                MsgHelper.MsgNotValidRangeTanggal();
                return;
            }

            LoadData(tanggalMulai, tanggalSelesai);
        }

        private void filterRangeTanggal_ChkTampilkanSemuaDataClicked(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;

            if (chk.Checked)
                LoadData();
            else
                LoadData(filterRangeTanggal.TanggalMulai, filterRangeTanggal.TanggalSelesai);
        }
    }
}
